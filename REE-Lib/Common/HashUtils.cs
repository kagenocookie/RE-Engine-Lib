using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace ReeLib.Common
{
    public static class HashUtils
    {
        private static bool _ignoreStringsLib = false;

        public static IEnumerable<string> ExtractStrings(Encoding encoding, Stream stream, SearchValues<char> charWhitelist, Func<string, bool>? filter = null)
            => ExtractStrings(encoding, stream, null, charWhitelist, filter);
        public static IEnumerable<string> ExtractStrings(Encoding encoding, string filepath, SearchValues<char> charWhitelist, Func<string, bool>? filter = null)
            => ExtractStrings(encoding, null, filepath, charWhitelist, filter);

        public static IEnumerable<string> ExtractStrings(Encoding encoding, Stream? stream, string? filepath, SearchValues<char> charWhitelist, Func<string, bool>? filter = null)
        {
            Debug.Assert(stream != null || File.Exists(filepath));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !_ignoreStringsLib && File.Exists("/usr/bin/strings")) {
                // prefer just using strings cause it's faster (sorry not sorry Windows)
                Process? proc = null;
                try {
                    var info = new ProcessStartInfo("/usr/bin/strings") {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = filepath == null,
                    };
                    info.ArgumentList.Add("-e");
                    info.ArgumentList.Add(encoding == Encoding.Unicode ? "l" : "S");
                    // it's still slightly faster to let it read the file on its own so add a separate path here for that case
                    if (filepath != null) {
                        info.ArgumentList.Add(filepath);
                    }
                    info.StandardOutputEncoding = Encoding.UTF8;
                    proc = Process.Start(info);
                } catch (Exception e) {
                    Log.Error("Failed to /usr/bin/strings, falling back to manual scan: " + e.Message);
                    _ignoreStringsLib = true;
                }

                if (proc != null) {
                    if (filepath == null) {
                        stream!.CopyToAsync(proc.StandardInput.BaseStream).ContinueWith(_ => proc.StandardInput.Close());
                    }
                    while (!proc.StandardOutput.EndOfStream) {
                        var line = proc.StandardOutput.ReadLine();
                        if (line != null && !line.ContainsAnyExcept(charWhitelist) && (filter == null || filter.Invoke(line))) {
                            yield return line;
                        }
                    }
                    yield break;
                }
            }

            // something like half as fast as strings, not _too_ horrible either
            if (stream == null) {
                using var sstr = File.OpenRead(filepath!);
                foreach (var item in ExtractStringsManually(sstr, encoding, charWhitelist, filter)) {
                    yield return item;
                }
            } else {
                foreach (var item in ExtractStringsManually(stream, encoding, charWhitelist, filter)) {
                    yield return item;
                }
            }
        }

        public static List<string> ExtractStringsManually(Stream stream, Encoding encoding, SearchValues<char> charWhitelist, Func<string, bool>? filter = null, int minMatchLength = 1)
        {
            var decoder = encoding.GetDecoder();
            var maxByteCount = encoding.GetMaxByteCount(8) - encoding.GetMaxByteCount(0); // negate null terminator
            var maxCharCount = encoding.GetMaxCharCount(maxByteCount);
            Span<byte> bytes = stackalloc byte[maxByteCount];
            Span<char> chars = stackalloc char[maxCharCount];
            var maxSlice = maxCharCount - 1;

            var sb = new StringBuilder();
            var strings = new List<string>();
            while (stream.Position < stream.Length) {
                var readBytes = stream.Read(bytes);
                var chCount = decoder.GetChars(bytes, chars, true);
                var validChars = chars.Slice(0, chCount).IndexOfAnyExcept(charWhitelist);

                if (validChars == -1) {
                    // chars includes includes a null terminator, we don't want it
                    sb.Append(chars.Slice(0, maxSlice));
                } else {
                    if (validChars > 0) {
                        sb.Append(chars.Slice(0, validChars));
                    }

                    if (validChars < chCount) {
                        if (sb.Length >= minMatchLength) {
                            var str = sb.ToString();
                            if (filter == null || filter.Invoke(str)) {
                                strings.Add(str);
                            }
                        }
                        sb.Clear();

                        var byteCount = validChars == -1 ? maxByteCount : encoding.GetByteCount(chars.Slice(0, validChars));
                        var zeros = bytes.IndexOfAnyExcept((byte)0);
                        var decodeCorrection = -readBytes + Math.Max(1, byteCount + zeros);
                        if (decodeCorrection != 0) stream.Seek(decodeCorrection, SeekOrigin.Current);
                    }
                }
            }

            // just in case we still had an unfinished string left over, see if it's a match
            if (sb.Length >= minMatchLength) {
                var str = sb.ToString();
                if (filter == null || filter.Invoke(str)) {
                    strings.Add(str);
                }
            }

            return strings;
        }
    }
}
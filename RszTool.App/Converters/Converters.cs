using System.Collections.ObjectModel;
using System.Windows.Data;
using RszTool.App.Common;
using RszTool.App.ViewModels;
using RszTool.Pfb;
using RszTool.Scn;

namespace RszTool.App.Converters
{
    [ValueConversion(typeof(RszInstance), typeof(ObservableCollection<object>))]
    public class RszInstanceFieldsConverter : IValueConverter
    {
        public static IEnumerable<object> GetItems(RszInstance instance)
        {
            if (instance.RSZUserData is RSZUserDataInfo userDataInfo)
            {
                yield return new ClassViewModel("UserDataInfo", userDataInfo, ["Path"]);
            }
            else if (instance.RSZUserData is RSZUserDataInfo_TDB_LE_67 userDataInfo_TDB_LE_67)
            {
                var embeddedRSZ = userDataInfo_TDB_LE_67.EmbeddedRSZ;
                if (embeddedRSZ != null &&
                    embeddedRSZ.ObjectList.Count > 0)
                {
                    foreach (var item in GetItems(embeddedRSZ.ObjectList[0]))
                    {
                        yield return item;
                    }
                }
            }
            else
            {
                for (int i = 0; i < instance.Values.Length; i++)
                {
                    var field = instance.RszClass.fields[i];
                    yield return field.array ?
                        new RszFieldArrayViewModel(instance, i) :
                        field.IsReference ? new RszFieldInstanceViewModel(instance, i) :
                                            new RszFieldNormalViewModel(instance, i);
                }
            }
        }

        public static ObservableCollection<object> Convert(RszInstance instance)
        {
            ObservableCollection<object> list = new(GetItems(instance));
            instance.ValuesChanged += sender => {
                list.Clear();
                foreach (var item in GetItems(sender))
                {
                    list.Add(item);
                }
            };
            return list;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var instance = (RszInstance)value;
            return Convert(instance);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }


    [ValueConversion(typeof(ScnGameObject), typeof(IEnumerable<object>))]
    public class ScnGameObjectSubItemsConverter : IValueConverter
    {
        public static IEnumerable<object> Convert(ScnGameObject gameObject)
        {
            if (gameObject.Instance != null)
            {
                yield return gameObject.Instance;
            }
            yield return new ClassViewModel("GameObjectInfo", gameObject, ["Guid"]);
            if (gameObject.Prefab != null)
            {
                yield return new ClassViewModel(gameObject.Prefab, ["Path"]);
            }
            yield return new TreeItemViewModel("Components",
                GameObejctComponentViewModel.MakeList(gameObject));
            yield return new TreeItemViewModel("Children", gameObject.Children);
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var gameObject = (ScnGameObject)value;
            return Convert(gameObject);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }


    [ValueConversion(typeof(ScnFolderData), typeof(IEnumerable<object>))]
    public class ScnFolderDataSubItemsConverter : IValueConverter
    {
        public static IEnumerable<object> Convert(ScnFolderData folder)
        {
            if (folder.Instance != null)
            {
                yield return folder.Instance;
            }
            yield return new TreeItemViewModel("Children", folder.Children);
            yield return new TreeItemViewModel("GameObjects", folder.GameObjects);
            yield return new TreeItemViewModel("Prefabs", folder.Prefabs);
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var folder = (ScnFolderData)value;
            return Convert(folder);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }


    [ValueConversion(typeof(PfbGameObject), typeof(IEnumerable<object>))]
    public class PfbGameObjectSubItemsConverter : IValueConverter
    {
        public static IEnumerable<object> Convert(PfbGameObject gameObject)
        {
            if (gameObject.Instance != null)
            {
                yield return gameObject.Instance;
            }
            yield return new TreeItemViewModel("Components",
                GameObejctComponentViewModel.MakeList(gameObject));
            yield return new TreeItemViewModel("Children", gameObject.Children);
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var gameObject = (PfbGameObject)value;
            return Convert(gameObject);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(via.Color), typeof(string))]
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((via.Color)value).Hex();
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return via.Color.Parse((string)value);
        }
    }


    [ValueConversion(typeof(Guid), typeof(string))]
    public class GuidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Guid)value).ToString();
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Guid.Parse((string)value);
        }
    }


    [ValueConversion(typeof(byte[]), typeof(string))]
    public class BytesConverter : IValueConverter
    {
        private int byteCount;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is byte[] bytes)
            {
                byteCount = bytes.Length;
                return ConvertUtils.ToHexString(bytes, " ");
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string str)
            {
                return ConvertUtils.FromHexString(str);
            }
            else
            {
                return Array.Empty<byte>();
            }
        }
    }


    [ValueConversion(typeof(object), typeof(Type))]
    public class TypeOfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    [ValueConversion(typeof(object), typeof(bool))]
    public class TypeIsAssignableFromConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter is not Type baseType)
            {
                throw new ArgumentException("Parameter must be a Type", nameof(parameter));
            }
            return baseType.IsAssignableFrom(value.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

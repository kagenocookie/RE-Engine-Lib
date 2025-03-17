using System.Collections.ObjectModel;
using System.Windows.Data;
using RszTool.App.Common;
using RszTool.App.ViewModels;

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


    [ValueConversion(typeof(ScnFile.GameObjectData), typeof(IEnumerable<object>))]
    public class ScnGameObjectDataSubItemsConverter : IValueConverter
    {
        public static IEnumerable<object> Convert(ScnFile.GameObjectData gameObject)
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
            var gameObject = (ScnFile.GameObjectData)value;
            return Convert(gameObject);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }


    [ValueConversion(typeof(ScnFile.FolderData), typeof(IEnumerable<object>))]
    public class ScnFolderDataSubItemsConverter : IValueConverter
    {
        public static IEnumerable<object> Convert(ScnFile.FolderData folder)
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
            var folder = (ScnFile.FolderData)value;
            return Convert(folder);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }


    [ValueConversion(typeof(PfbFile.GameObjectData), typeof(IEnumerable<object>))]
    public class PfbGameObjectDataSubItemsConverter : IValueConverter
    {
        public static IEnumerable<object> Convert(PfbFile.GameObjectData gameObject)
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
            var gameObject = (PfbFile.GameObjectData)value;
            return Convert(gameObject);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(RcolFile.RcolGroupInfo), typeof(ObservableCollection<object>))]
    public class RcolInfoFieldsConverter : IValueConverter
    {
        public static IEnumerable<object> GetItems(RcolFile.RcolGroupInfo instance)
        {
            yield return new ClassViewModel("Info", instance, ["name", "guid", "layerGuid", "layerIndex", "maskBits"]);
        }

        public static ObservableCollection<object> Convert(RcolFile.RcolGroupInfo instance)
        {
            ObservableCollection<object> list = new(GetItems(instance));
            // instance.ValuesChanged += sender => {
            //     list.Clear();
            //     foreach (var item in GetItems(sender))
            //     {
            //         list.Add(item);
            //     }
            // };
            return list;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Convert((RcolFile.RcolGroupInfo)value);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(RcolFile.RcolGroup), typeof(IEnumerable<object>))]
    public class RcolGroupShapesConverter : IValueConverter
    {
        public static IEnumerable<object> Convert(RcolFile.RcolGroup group)
        {
            yield return new ClassViewModel("Info", group.Info, ["Name", "Guid", "LayerIndex", "MaskBits", "LayerGuid"]);
            yield return new TreeItemViewModel("Shapes", RcolShapeViewModel.MakeList(group));
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var gameObject = (RcolFile.RcolGroup)value;
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

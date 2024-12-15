using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace RestAPI_Base.Models
{
    public class ResponseModel<T>
    {
        public int code { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public T data { get; set; }
        public ResponseModel(int code, string status, string message, T data)
        {
            this.code = code;
            this.status = status;
            this.message = message;
            this.data = data;

            CheckFields(data);
        }

        private void CheckFields(object obj)
        {
            try
            {
                if (obj == null) return;

                Type type = obj.GetType();

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var list = obj as IEnumerable<object>;
                    foreach (var item in list)
                    {
                        CheckFields(item);
                    }
                }
                else if (obj is IDictionary dictionary)
                {
                    return;
                }
                else
                {
                    var properties = type.GetProperties();
                    foreach (var property in properties)
                    {
                        try
                        {
                            object value = property.GetValue(obj);

                            Type propertyType = property.PropertyType;
                            if (propertyType == typeof(string))
                            {
                                if (value == null) property.SetValue(obj, "");
                            }
                            else if (propertyType == typeof(int) || propertyType == typeof(long) || propertyType == typeof(double))
                            {
                                if (value == null) property.SetValue(obj, 0);
                            }
                            else if (propertyType == typeof(DateTime))
                            {
                                continue;
                            }
                            else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                Type listType = propertyType.GetGenericArguments()[0];
                                if (listType == typeof(string))
                                {
                                    if (value == null) property.SetValue(obj, new List<string>());
                                }
                                else
                                {
                                    if (value == null)
                                    {
                                        property.SetValue(obj, Activator.CreateInstance(propertyType));
                                    }
                                    else
                                    {
                                        var list = value as IEnumerable<object>;
                                        if (list == null) continue;
                                        foreach (var item in list)
                                        {
                                            CheckFields(item);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"{property.Name} : {property.GetValue(obj)}, {propertyType}");
                            }
                        }
                        catch (Exception ex) 
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}

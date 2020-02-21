using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonLibrary
{
    public class JsonManager
    {
        public JsonManager()
        {
            JsonConvert.DefaultSettings = () => DefaultSettings;
        }
        public JsonManager(JsonSerializerSettings settings)
        {
            DefaultSettings = settings;
            JsonConvert.DefaultSettings = () => DefaultSettings;
        }

        public JsonSerializerSettings DefaultSettings { get; set; }

        public T DeserializeJson<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<T> DeserializeJsonAsync<T>(string json)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (Exception e)
                {
                    throw;
                }
            });
        }
        public async Task<T> DeserializeJsonAsync<T>(string json, string propertyName, string newValue)
        {
            JObject jObject = JObject.Parse(json);
            jObject.Property(propertyName).Value = newValue;

            T obj = await Task.Run(() =>
            {
                try
                {
                    return jObject.ToObject<T>(); //JsonConvert.DeserializeObject<T>(jObject);
                }
                catch (Exception e)
                {
                    throw;
                }
            });

            return obj;
        }
        public async Task<T> DeserializeJsonFileAsync<T>(string fullPath)
        {
            using (var file = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true))
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                byte[] buffer = new byte[0x1000];
                int numRead;
                while ((numRead = await file.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string text = System.Text.Encoding.UTF8.GetString(buffer, 0, numRead);
                    sb.Append(text);
                }

                return JsonConvert.DeserializeObject<T>(sb.ToString());
            }
        }
        public string SerializeToJsonInMemory<T>(T obj)
        {
            try
            {
                string json = JsonConvert.SerializeObject(obj);
                return json;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<string> SerializeToJsonInMemoryAsync<T>(T obj)
        {
            return await Task.Run(() =>
            {
                try
                {
                    string json = JsonConvert.SerializeObject(obj);
                    return json;
                }
                catch(Exception e)
                {
                    throw;
                }
            });
        }
        public async Task SerializeToJsonInFile<T>(T obj, string fullPath)
        {
            using (var file = File.Open(fullPath, FileMode.Create))
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream))
                    {
                        var serializer = JsonSerializer.CreateDefault();

                        serializer.Serialize(writer, obj);

                        await writer.FlushAsync().ConfigureAwait(false);

                        memoryStream.Seek(0, SeekOrigin.Begin);

                        await memoryStream.CopyToAsync(file).ConfigureAwait(false);
                    }
                }

                await file.FlushAsync().ConfigureAwait(false);
            }
        }
        public async Task<string> SerializeToJsonInMemory_ReadableAsync<T>(T obj)
        {
            return await Task.Run(() =>
            {
                try
                {
                    string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                    return json;
                }
                catch (Exception e)
                {
                    throw;
                }
            });
        }
        public async Task SerializeToJsonInFile_ReadableAsync<T>(T obj, string fullPath)
        {
            try
            {
                using (var file = File.Open(fullPath, FileMode.Create))
                {
                    using (var memoryStream = new MemoryStream())
                    using (var writer = new StreamWriter(memoryStream))
                    {
                        var serializer = JsonSerializer.CreateDefault();
                        serializer.Formatting = Formatting.Indented;

                        serializer.Serialize(writer, obj);

                        await writer.FlushAsync().ConfigureAwait(false);

                        memoryStream.Seek(0, SeekOrigin.Begin);

                        await memoryStream.CopyToAsync(file).ConfigureAwait(false);
                    }

                    await file.FlushAsync().ConfigureAwait(false);
                }
            }
            catch(Exception e)
            {
                throw;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Serialization
{
    internal static class QuickStart
    {
        public static void Main_()
        {
            // Создание графа объектов для последующей сериализации в поток
            var objectGraph = new List<String> {"Jeff", "Kristin", "Aidan", "Grant" };
            Stream stream = SerializeToMemory(objectGraph);
            
            // Обнуляем все для данного примера
            stream.Position = 0;
            objectGraph = null;

            // Десериализация объектов и проверка их работоспособности
            objectGraph = (List<String>)DeserializeFromMemory(stream);
            foreach (var s in objectGraph)
            {
                Console.WriteLine(s);
            }
        }

        private static MemoryStream SerializeToMemory(Object objectGraph)
        {
            // Конструирование потока, который будет содержать сериализованные объекты
            MemoryStream stream = new MemoryStream();

            // Задание форматирования при сериализации
            BinaryFormatter formatter = new BinaryFormatter();

            // Заставляем модуль форматирования сериализовать объекты в поток
            formatter.Serialize(stream, objectGraph);

            // Возвращение потока сериализованных объектов вызывающему методу
            return stream;
        }

        private static Object DeserializeFromMemory(Stream stream)
        {
            // Задание форматирования при сериализации
            BinaryFormatter formatter = new BinaryFormatter();

            // Заставляем модуль форматирования десериализовать объекты из потока
            return formatter.Deserialize(stream);
        }
    }
}

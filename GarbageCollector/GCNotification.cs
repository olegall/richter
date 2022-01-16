using System;
using System.Threading;

namespace GarbageCollector
{
    public static class GCNotification
    {
        private static Action<Int32> s_gcDone = null; // Поле события
        
		public static event Action<Int32> GCDone
        {
            add
            {
                // Если зарегистрированные делегаты отсутствуют, начинаем оповещение
                if (s_gcDone == null) 
				{ 
					new GenObject(0); 
					new GenObject(2); 
				}

                s_gcDone += value;
            }

            remove 
			{ 
				s_gcDone -= value; 
			}
        }

		private sealed class GenObject
		{
			private Int32 m_generation;

			public GenObject(Int32 generation)
			{
				m_generation = generation;
			}

			~GenObject()
			{ 
				// Метод финализации. Если объект принадлежит нужному нам поколению (или выше), оповещаем делегат о выполненной уборке мусора
				Action<Int32> temp = Volatile.Read(ref s_gcDone);
				
				if (temp != null) 
					temp(m_generation);
			}

			void Foo() // мой метод, скорее всего пропущен в Рихтере. Без него всё ломается
			{
				// Продолжаем оповещение, пока остается хоть один зарегистрированный делегат, домен приложений не выгружен и процесс не завершен
				if ((s_gcDone != null) && !AppDomain.CurrentDomain.IsFinalizingForUnload() && !Environment.HasShutdownStarted)
				{
					// Для поколения 0 создаем объект; для поколения 2 воскрешаем объект и позволяем уборщику вызвать метод финализации при следующей уборке мусора для поколения 2
					if (m_generation == 0)
						new GenObject(0);
					else
						GC.ReRegisterForFinalize(this);
				}
				else
				{
					/* Позволяем объекту исчезнуть */
				}
			}
		}
	}
}
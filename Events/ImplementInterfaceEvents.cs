using System;

// microsoft
namespace ImplementInterfaceEvents
{
    public interface IDrawingObject
    {
        event EventHandler ShapeChanged;
    }

    public class MyEventArgs : EventArgs
    {
        // class members  
    }

    public class Shape : IDrawingObject
    {
        public event EventHandler ShapeChanged; // как в интерфейсе. что изменилось? убрать из интерфейса - ничего не изменится. aleek

        void ChangeShape()
        {
            // Do something here before the event…  
            OnShapeChanged(new MyEventArgs(/*arguments*/));
            // or do something here after the event.
        }

        protected virtual void OnShapeChanged(MyEventArgs e)
        //protected void OnShapeChanged(MyEventArgs e) // ok
        {
            // что будет, если ShapeChanged = null?
            ShapeChanged?.Invoke(this, e);
        }
    }
}

namespace Reflection
{
    #region aleek
    class Foo
    {
        public int FooProp { get; set; }
    }

    internal sealed class SomeTypeRefTest
    {
        public SomeTypeRefTest(Foo fooProp)
        {
            fooProp.FooProp *= 3;
        }
    }
    #endregion
}
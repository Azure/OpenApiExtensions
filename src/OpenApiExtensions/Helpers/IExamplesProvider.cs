namespace Microsoft.Azure.OpenApiExtensions.Helpers
{
    public interface IExamplesProvider
    {
        object GetExample();
    }

    public abstract class BodyExamplesProvider : IExamplesProvider
    {
        public object GetExample()
        {
            return new { body = GetBodyExample() };
        }

        protected abstract object GetBodyExample();
    }

    public abstract class BaseExamplesProvider<TInnerValue> : IExamplesProvider
    {
        public abstract ResponseObj<TInnerValue> GetTypedExamples();

        public object GetExample()
        {
            return GetTypedExamples();
        }
    }
    public class ResponseObj<TInnerValue>
    {
        public BodyType<TInnerValue> Body { get; set; }

        public class BodyType<TInnerBodyValue>
        {
            public TInnerBodyValue Value { get; set; }
        }
    }
}

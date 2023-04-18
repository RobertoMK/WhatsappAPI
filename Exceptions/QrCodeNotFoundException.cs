namespace WhatsappAPI.Exceptions
{
    public class QrCodeNotFoundException : Exception
    {
        private readonly string message;
        public QrCodeNotFoundException(string m) {
            message = m;
        }
        
        public override string ToString() { return message; }
    }
}

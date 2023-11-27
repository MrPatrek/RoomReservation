namespace EmailService.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
    }
}

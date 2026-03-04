using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace RegIN_Markov.Classes
{
    public class SendMail
    {
        public static void SendMessage(string Message, string To)
        {
            var smtpClient = new SmtpClient("smtp.yandex.ru")
            {
                Port = 587,
                Credentials = new NetworkCredential("landaxer@yandex.ru", "password"), // Исправлено: согласован отправитель
                EnableSsl = true,
            };
            smtpClient.Send("landaxer@yandex.ru", To, "Проект RegIn", Message); // Исправлено: согласован отправитель
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using CsQuery.ExtensionMethods.Internal;
using Newtonsoft.Json;

namespace InfoSearcher
{
    public class DataSender
    {
        private const string HtmlTemplateFileName = "data_{0}.html";
        private readonly MainSettings _mainSettings;

        public DataSender(MainSettings mainSettings)
        {
            _mainSettings = mainSettings ?? throw new ArgumentNullException(nameof(mainSettings));
        }

        public void SendData()
        {
            // 1 collect data from output
            var dataFiles = Directory.GetFiles(_mainSettings.OutputDir, "*.dat", SearchOption.AllDirectories);
            var onlyNewData = dataFiles.Where(x =>
                Path.GetDirectoryName(x).Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
                    .Last().ToLowerInvariant() != "Archive");

            var datas = onlyNewData.Select(x =>
            {
                var content = File.ReadAllText(x);
                var data = JsonConvert.DeserializeObject<OutPutData>(content);
                return data;
            }).ToList();

            // 2 send mail
            var tempFolder = Path.GetTempPath() + Guid.NewGuid().ToString("N");
            try
            {
                var attachmentFiles = new List<string>();
                Directory.CreateDirectory(tempFolder);
                for (var i = 0; i < datas.Count; i++)
                {
                    var htmlFileName = Path.Combine(tempFolder, string.Format(HtmlTemplateFileName, i));
                    File.WriteAllText(htmlFileName, datas[i].Content);
                    attachmentFiles.Add(htmlFileName);
                }

                var bodyHtmlFileName = Path.Combine(tempFolder, "body.html");
                var body = CreateBody(datas, _mainSettings.BodyHtmlTemplate);
                File.WriteAllText(bodyHtmlFileName, body);
                SendEmail(_mainSettings, bodyHtmlFileName, attachmentFiles);
            }
            finally
            {
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }

            // 3 move sent data to archive folder
        }

        private static void SendEmail(MainSettings mainSettings, string bodyFile, IEnumerable<string> attachments)
        {
            using (var client = new SmtpClient())
            {
                var message = new MailMessage
                {
                    From = new MailAddress(mainSettings.From),
                    IsBodyHtml = true,
                    Body = File.ReadAllText(bodyFile)
                };

                var toAddresses = mainSettings.To.Select(x => new MailAddress(x));
                message.To.AddRange(toAddresses);
                var mailAttachments = attachments.Select(x => new Attachment(x));
                message.Attachments.AddRange(mailAttachments);

                client.Host = mainSettings.SmtpServer;
                client.Port = mainSettings.SmtpPort ?? 25;
                client.EnableSsl = mainSettings.SmtpUseSsl ?? false;
                if (!string.IsNullOrEmpty(mainSettings.SmtpLogin) && !string.IsNullOrEmpty(mainSettings.SmtpPass))
                {
                    client.Credentials = new NetworkCredential(mainSettings.SmtpLogin, mainSettings.SmtpPass);
                }

                client.Send(message);
            }
        }

        private static string CreateBody(IReadOnlyList<OutPutData> datas, string bodyHtmlTemplate)
        {
            var content = new StringBuilder();
            for (var i = 0; i < datas.Count; i++)
            {
                var uri = new Uri(datas[i].MainLink);
                var fileName = string.Format(HtmlTemplateFileName, i);
                var row = $"<tr><td>{uri.Host}</td>" +
                          $"<td><a href=\"cid:{fileName}\">{fileName}</a></td>" +
                          $"<td><a href=\"{datas[i].Url}\"/>Новость</td>" +
                          $"<td>{datas[i].DownLoadDate}</td>" +
                          "</tr>";
                content.Append(row);
            }

            var template = File.ReadAllText(bodyHtmlTemplate);
            var body = template.Replace("{DATA-PLACE}", content.ToString());
            return body;
        }
    }
}
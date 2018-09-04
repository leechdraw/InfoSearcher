using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using AegisImplicitMail;
using CsQuery.ExtensionMethods.Internal;
using Newtonsoft.Json;

namespace InfoSearcher
{
    public class DataSender
    {
        private const string HtmlTemplateFileName = "data_{0}.html";
        private const string ArchiveFolderName = "Archive";
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
                    .Last().ToLowerInvariant() != ArchiveFolderName)
                .ToList();

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
                var body = CreateBody(datas, _mainSettings.Mail.BodyHtmlTemplate);
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
            foreach (var file in onlyNewData)
            {
                var fileName = Path.GetFileName(file);
                var dir = Path.GetDirectoryName(file);
                var archiveDir = Path.Combine(dir, ArchiveFolderName);
                if (!Directory.Exists(archiveDir))
                {
                    Directory.CreateDirectory(archiveDir);
                }

                var archiveFileName = Path.Combine(archiveDir, fileName);
                File.Move(file, archiveFileName);
            }
        }

        private static void SendEmail(MainSettings mainSettings, string bodyFile, IEnumerable<string> attachments)
        {
            using (var client = new SmtpSocketClient())
            {
                var message = new MimeMailMessage
                {
                    From = new MimeMailAddress(mainSettings.Mail.From),
                    IsBodyHtml = true,
                    Body = File.ReadAllText(bodyFile),
                    Subject = mainSettings.Mail.Subject
                };

                var toAddresses = mainSettings.Mail.To.Select(x => new MimeMailAddress(x));
                message.To.AddRange(toAddresses);
                var mailAttachments = attachments.Select(x => new MimeAttachment(x));
                message.Attachments.AddRange(mailAttachments);
                
                client.Host = mainSettings.Mail.SmtpServer;
                client.Port = 465;
               
                    client.User = mainSettings.Mail.SmtpLogin;
                    client.Password = mainSettings.Mail.SmtpPass;

               

                client.SslType = SslMode.Ssl;
                client.AuthenticationMode = AuthenticationType.Base64;
                
                client.SendMail(message);
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
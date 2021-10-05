using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery
{
	public class EmailConfig
	{

		public string MailServer { get; set; } = "mail.delphipm.org";
		public int MailPort { get; set; } = 25;
		public string SenderName { get; set; }
		public string Sender { get; set; }
		public string Password { get; set; }
	}
}

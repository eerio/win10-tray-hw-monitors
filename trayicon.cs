using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Management;


public class Program
{
    [STAThread]
    static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		ProcessStartInfo start = new ProcessStartInfo();
		start.WindowStyle = ProcessWindowStyle.Hidden;
		
		TrayIcon batteryIcon = new TrayIcon("temp");
		TrayIcon memoryIcon = new TrayIcon("memory");
		TrayIcon cpuIcon = new TrayIcon("cpu");

		Application.Run();
	}
		
    class TrayIcon
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool DestroyIcon(IntPtr handle);

        private const string iconFont = "Segoe UI";
        private const int iconFontSize = 14;

        private NotifyIcon notifyIcon;
		
		private ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
		private Double temperature;
		private String instanceName;
		private PerformanceCounter pcc;
		private string typeOfIcon;
		private string stringToDisplay;
		
        public TrayIcon(string _typeOfIcon)
        {
			temperature=0;
			instanceName="";
			typeOfIcon = _typeOfIcon;
			
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();

            notifyIcon = new NotifyIcon();

            // initialize contextMenu
            contextMenu.MenuItems.AddRange(new MenuItem[] { menuItem });

            // initialize menuItem
            menuItem.Index = 0;
            menuItem.Text = "E&xit";
            menuItem.Click += new System.EventHandler(menuItem_Click);

            notifyIcon.ContextMenu = contextMenu;
			
			if (string.Equals(typeOfIcon, "memory"))
			{
				pcc = new PerformanceCounter("Pamięć", "Zadeklarowane bajty w użyciu (%)");
			}
			else if (string.Equals(typeOfIcon, "cpu"))
			{
				pcc = new PerformanceCounter("Procesor", "Czas procesora (%)", "_Total");
			}

            notifyIcon.Visible = true;
			
            Timer timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 500; // in miliseconds
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
			double val=0;
	
			if (string.Equals(typeOfIcon, "temp"))
			{
				foreach (ManagementObject obj in searcher.Get())
				{
				temperature = Convert.ToDouble(obj["CurrentTemperature"].ToString());
				// Convert the value to celsius degrees
				temperature = (temperature - 2732) / 10.0;
				instanceName = obj["InstanceName"].ToString();
				}
				
				val = temperature;
				stringToDisplay = "Temp: " + temperature.ToString() + "C";
			}
			else if (string.Equals(typeOfIcon, "memory"))
			{
				val = Math.Round(pcc.NextValue());
				stringToDisplay = "Mem: " + Math.Round(pcc.NextValue()).ToString() + "%";
			}
			else if (string.Equals(typeOfIcon, "cpu"))
			{
				val = Math.Round(pcc.NextValue());
				stringToDisplay = "CPU: " + Math.Round(pcc.NextValue()).ToString() + "%";
			}
			
			Color[] colors = {Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Purple, Color.Pink};
			int interval = (int)Math.Floor(val / (100 / colors.Length));
			Color bg_col = colors[interval % 7];
			
			
            using (Bitmap bitmap = new Bitmap(DrawText(val.ToString(), new Font(iconFont, iconFontSize), bg_col)))
            {
                System.IntPtr intPtr = bitmap.GetHicon();
                try
                {
                    using (Icon icon = Icon.FromHandle(intPtr))
                    {
                        notifyIcon.Icon = icon;
                        notifyIcon.Text = stringToDisplay;
                    }
                }
                finally
                {
                    DestroyIcon(intPtr);
                }
            }
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Application.Exit();
        }

        private Image DrawText(String text, Font font, Color backColor)
        {
			Color textColor = Color.FromArgb(backColor.ToArgb()^0xFFFFFF);
            var textSize = GetImageSize(text, font);
            Image image = new Bitmap((int) textSize.Width, (int) textSize.Height);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                // paint the background
                graphics.Clear(backColor);

                // create a brush for the text
                using (Brush textBrush = new SolidBrush(textColor))
                {
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    graphics.DrawString(text, font, textBrush, 0, 0);
                    graphics.Save();
                }
            }

            return image;
        }

        private static SizeF GetImageSize(string text, Font font)
        {
            using (Image image = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(image))
                return graphics.MeasureString(text, font);
        }
    }
}
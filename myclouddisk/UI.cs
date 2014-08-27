using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
namespace myclouddisk
{
    class UI
    {
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem item1;
        private System.Windows.Forms.ToolStripMenuItem item2;

        private String path;
        private Icon iNotify = new Icon(@"images/notify.ico");
        private Icon iNotifySync = new Icon(@"images/sync.ico");

        public UI(String path)
        {
            this.path = path;
            init();
        }
        private void init()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();

            this.menu = new System.Windows.Forms.ContextMenuStrip();

            this.item1 = new System.Windows.Forms.ToolStripMenuItem();
            this.item2 = new System.Windows.Forms.ToolStripMenuItem();
     
            // 
            // notifyIcon
            // 
            notifyIcon.ContextMenuStrip = this.menu;
            notifyIcon.Text = "程序正在初始化中，请稍后使用";
           
            notifyIcon.Icon = iNotify;
            this.notifyIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(notify_Click);//为系统托盘添加鼠标事件监听
            this.notifyIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(notify_Move);//添加鼠标飘过事件
           
            notifyIcon.Visible = true;
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] 
            {
                this.item1,this.item2
            });
           
            this.menu.Name = "menu";
            // 
            // item1
            // 
            this.item1.Name = "item1";
            this.item1.Text = "在文件资源管理器中打开";
            this.item1.MouseDown += new System.Windows.Forms.MouseEventHandler(item1_Click);
            //
            //item2
            //
            this.item2.Name = "item2";
            this.item2.Text = "选项";
            this.item2.MouseDown += new System.Windows.Forms.MouseEventHandler(item2_Click);
     
        }
        private void notify_Click(object sender, MouseEventArgs e)
        {
            
           
            if (e.Button == MouseButtons.Left)
            {
                if(Directory.Exists(this.path))
                System.Diagnostics.Process.Start(path);
            }
            else if(e.Button == MouseButtons.Right)
            {
                menu.Visible = true;              
            }
        }
        private void notify_Move(object sender, MouseEventArgs e)
        {
            if (Program.status == RsyncStatus.FINISHED)
            {
                TimeSpan ts1 = new TimeSpan(Program.lastUpdateTime.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                string timeDiff;
                if (ts.Hours > 0)
                {
                    timeDiff = ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟前";
                }
                else if (ts.Minutes > 1)
                {
                    timeDiff = ts.Minutes.ToString() + "分钟前";
                }
                else
                    timeDiff = "刚刚更新";
                this.notifyIcon.Text = "文件是最新的 - 上次更新时间：" + timeDiff;

            }
            else if (Program.status == RsyncStatus.STARTING)
                this.notifyIcon.Text = "程序正在启动中";
            else if (Program.status == RsyncStatus.RSYNCING)
                this.notifyIcon.Text = "文件正在同步中";
            else
                this.notifyIcon.Text = "无法获取程序状态";

        }
        private void item1_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Directory.Exists(this.path))
                    System.Diagnostics.Process.Start(path);
            }
        }
        private void item2_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                new OpinionWindow().Visible=true;
            }
        }
        public void setIcon(String name)
        {
            switch(name)
            {
                case "sync": this.notifyIcon.Icon = this.iNotifySync; break;
                default: this.notifyIcon.Icon = this.iNotify; break;
            }
        }

    }
}

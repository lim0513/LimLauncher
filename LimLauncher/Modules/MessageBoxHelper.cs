using ModernMessageBoxLib;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static ModernMessageBoxLib.QModernMessageBox;

namespace LimLauncher.Modules
{
    public class MessageBoxHelper
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        static MessageBoxHelper()
        {
            QModernMessageBox.MainLang = new QMetroMessageLang()
            {
                Ok = "确定",
                Cancel = "取消",
                Abort = "中止(A)",
                Ignore = "忽略(I)",
                No = "否(N)",
                Yes = "是(Y)",
                Retry = "重试(R)"
            };
        }

        /// <summary>
        /// 全局背景色
        /// </summary>
        public static Brush GlobalBackground
        {
            get { return QModernMessageBox.GlobalBackground; }
            set { QModernMessageBox.GlobalBackground = value; }
        }

        /// <summary>
        /// 全局前景色
        /// </summary>
        public static Brush GlobalForeground
        {
            get { return QModernMessageBox.GlobalForeground; }
            set { QModernMessageBox.GlobalForeground = value; }
        }

        /// <summary>
        /// 全局父窗体
        /// </summary>
        public static Window GlobalParentWindow
        {
            get { return QModernMessageBox.GlobalParentWindow; }
            set { QModernMessageBox.GlobalParentWindow = value; }
        }

        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="Message">消息内容</param>
        /// <param name="Title">标题</param>
        /// <param name="Btns">按钮类型</param>
        /// <param name="Icon">图标类型</param>
        /// <param name="PlaySound">播放声音</param>
        /// <returns></returns>
        public static ModernMessageboxResult ShowMessageBox(
            string Message,
            string Title,
            QModernMessageBoxButtons Btns = QModernMessageBoxButtons.Ok,
            ModernMessageboxIcons Icon = ModernMessageboxIcons.None,
            bool PlaySound = true)
                => ShowMessageBox(GlobalParentWindow, Message, Title, Btns, Icon, PlaySound);

        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="ParentWindow">父窗体</param>
        /// <param name="Message">消息内容</param>
        /// <param name="Title">标题</param>
        /// <param name="Btns">按钮类型</param>
        /// <param name="Icon">图标类型</param>
        /// <param name="PlaySound">播放声音</param>
        /// <returns></returns>
        public static ModernMessageboxResult ShowMessageBox(
            Window ParentWindow,
            string Message,
            string Title,
            QModernMessageBoxButtons Btns = QModernMessageBoxButtons.Ok,
            ModernMessageboxIcons Icon = ModernMessageboxIcons.None,
            bool PlaySound = true)
                => QModernMessageBox.Show(ParentWindow, Message, Title, Btns, Icon, PlaySound);

        /// <summary>
        /// 错误消息框
        /// </summary>
        /// <param name="Ex"></param>
        public static void ShowErrorMessage(Exception Ex) => QModernMessageBox.Error(Ex.Message, "错误");

        public static void ShowErrorMessage(Window parentWindow, Exception Ex) => QModernMessageBox.Error(parentWindow, Ex.Message, "错误");

        /// <summary>
        /// 错误消息框
        /// </summary>
        /// <param name="Message"></param>
        public static void ShowErrorMessage(string Message) => QModernMessageBox.Error(Message, "错误");
        public static void ShowErrorMessage(Window parentWindow, string Message) => QModernMessageBox.Error(parentWindow, Message, "错误");
        /// <summary>
        /// 警告消息框
        /// </summary>
        /// <param name="Message"></param>
        public static void ShowWarningMessage(string Message) => QModernMessageBox.Warning(Message, "警告");

        public static void ShowWarningMessage(Window parentWindow, string Message) => QModernMessageBox.Warning(parentWindow, Message, "警告");

        /// <summary>
        /// 信息消息框
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Title"></param>
        public static void ShowInformationMessage(string Message, string Title) => QModernMessageBox.Info(Message, Title);
        public static void ShowInformationMessage(Window parentWindow, string Message, string Title) => QModernMessageBox.Info(Message, Title);

        /// <summary>
        /// 确定，取消消息框
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Title"></param>
        /// <returns></returns>
        public static ModernMessageboxResult ShowOKCancelMessage(string Message, string Title) => ShowMessageBox(Message, Title, QModernMessageBoxButtons.OkCancel, ModernMessageboxIcons.Question);

        /// <summary>
        /// 等待时处理
        /// </summary>
        /// <param name="ParentWindow">父窗体</param>
        /// <param name="WaitWindow">等待窗体</param>
        public delegate void DoWork(object ParentWindow, IndeterminateProgressWindow WaitWindow);

        /// <summary>
        /// 显示等待消息
        /// </summary>
        /// <param name="Work">等待时后台任务</param>
        /// <param name="WaitMessage">等待消息</param>
        /// <param name="EndMessage">结束消息</param>
        /// <param name="ParentWindow">父窗体</param>
        public static void ShowProgressMessage(DoWork Work, string WaitMessage, Window ParentWindow)
        {
            IndeterminateProgressWindow win = new IndeterminateProgressWindow(WaitMessage) { Owner = ParentWindow };
            Task.Run(() =>
            {
                Work(ParentWindow, win);
                ParentWindow.Dispatcher.BeginInvoke(new Action(() => { win.Close(); }));
            });
            win.ShowDialog();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace Reminder
{
    public class AutoStartHelper
    {
        /// <summary>
        /// ����������Ϊ��������
        /// </summary>
        /// <param name="onOff">��������</param>
        /// <returns></returns>
        public static bool SetMeStart(bool onOff)
        {
            bool isOk = false;
            string appName = Process.GetCurrentProcess().MainModule.ModuleName;
            string appPath = Process.GetCurrentProcess().MainModule.FileName;
            isOk = SetAutoStart(onOff, appName, appPath);
            return isOk;
        }

        /// <summary>
        /// ��Ӧ�ó�����Ϊ����Ϊ��������
        /// </summary>
        /// <param name="onOff">��������</param>
        /// <param name="appName">Ӧ�ó�����</param>
        /// <param name="appPath">Ӧ�ó�����ȫ·��</param>
        public static bool SetAutoStart(bool onOff, string appName, string appPath)
        {
            bool isOk = true;
            //�����û����Ϊ�����������õ�Ҫ��Ϊ��������
            if (!IsExistKey(appName) && onOff)
            {
                isOk = SelfRunning(onOff, appName, @appPath);
            }
            //�������Ϊ�����������õ���Ҫ��Ϊ��������
            else if (IsExistKey(appName) && !onOff)
            {
                isOk = SelfRunning(onOff, appName, @appPath);
            }
            return isOk;
        }

        /// <summary>
        /// �ж�ע���ֵ���Ƿ���ڣ����Ƿ��ڿ�������״̬
        /// </summary>
        /// <param name="keyName">��ֵ��</param>
        /// <returns></returns>
        public static bool IsExistKey(string keyName)
        {
            try
            {
                bool _exist = false;
                RegistryKey local = Registry.LocalMachine;
                RegistryKey runs = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (runs == null)
                {
                    RegistryKey key2 = local.CreateSubKey("SOFTWARE");
                    RegistryKey key3 = key2.CreateSubKey("Microsoft");
                    RegistryKey key4 = key3.CreateSubKey("Windows");
                    RegistryKey key5 = key4.CreateSubKey("CurrentVersion");
                    RegistryKey key6 = key5.CreateSubKey("Run");
                    runs = key6;
                }
                string[] runsName = runs.GetValueNames();
                foreach (string strName in runsName)
                {
                    if (strName.ToUpper() == keyName.ToUpper())
                    {
                        _exist = true;
                        return _exist;
                    }
                }
                return _exist;

            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// д���ɾ��ע����ֵ��,����Ϊ���������򿪻�������
        /// </summary>
        /// <param name="isStart">�Ƿ񿪻�����</param>
        /// <param name="exeName">Ӧ�ó�����</param>
        /// <param name="path">Ӧ�ó���·����������</param>
        /// <returns></returns>
        private static bool SelfRunning(bool isStart, string exeName, string path)
        {
            try
            {
                RegistryKey local = Registry.LocalMachine;
                RegistryKey key = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key == null)
                {
                    local.CreateSubKey("SOFTWARE//Microsoft//Windows//CurrentVersion//Run");
                }
                //����������������Ӽ�ֵ��
                if (isStart)
                {
                    key.SetValue(exeName, path);
                    key.Close();
                }
                else//����ɾ����ֵ��
                {
                    string[] keyNames = key.GetValueNames();
                    foreach (string keyName in keyNames)
                    {
                        if (keyName.ToUpper() == exeName.ToUpper())
                        {
                            key.DeleteValue(exeName);
                            key.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ss = ex.Message;
                return false;
                //throw;
            }

            return true;
        }
    }
}

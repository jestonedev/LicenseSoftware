using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Globalization;

namespace CryptDecryptAes
{
    public partial class FormCrypt : Form
    {
        string key = "Z33wYQs+rZOGMUH0RGj8GIKggOB82xwhACg9JCA6/kw=";
        string IV = "E0Ci3P9JPj43jUdKNSvtmQ==";
        Aes aes = Aes.Create();
        
        public FormCrypt()
        {
            InitializeComponent();
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(IV);
        }

        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
           byte[] inputBuffer = Encoding.UTF8.GetBytes(textBoxInput.Text);
           var encryptor = aes.CreateEncryptor();
           byte[] outputBuffer = encryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
           textBoxOutput.Text = Convert.ToBase64String(outputBuffer);
        }

        private void buttonDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] inputBuffer = Convert.FromBase64String(textBoxOutput.Text);
                var decryptor = aes.CreateDecryptor();
                byte[] outputBuffer = decryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                textBoxInput.Text = Encoding.UTF8.GetString(outputBuffer);
            }
            catch (CryptographicException)
            {
                MessageBox.Show("Входная строка не является корректным шифром", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException)
            {
                MessageBox.Show("Входная строка не является корректным шифром", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonEncrypt_Click(sender, new EventArgs());
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxOutput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonDecrypt_Click(sender, new EventArgs());
                e.SuppressKeyPress = true;
            }
        }
    }
}

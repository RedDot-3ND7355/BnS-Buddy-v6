using bnstool;
using BnSBuddy2.Pilao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BnSBuddy2.Pilao
{
    public class configurator
    { 
        public configurator()
        {
            // Initialize virtual elements
            Use64Bit.Checked = true;
            UseCompression.Checked = true;
            UseEncryption.Checked = true;
            UseSignature.Checked = true;
            // Fill AES Keys
            Encryption.Items.Add(new AESKeyObject(new byte[16] { 23, 81, 170, 213, 30, 54, 74, 27, 254, 96,116, 231, 208, 133, 7, 104 }));
            Encryption.Items.Add(new AESKeyObject("ja#n_2@020_compl"));
            Encryption.Items.Add(new AESKeyObject("jan_2#0_cpl_bns!"));
            Encryption.Items.Add(new AESKeyObject("bns_obt_kr_2014#"));
            Encryption.Items.Add(new AESKeyObject("bns_fgt_cb_2010!"));
            Encryption.Items.Add(new AESKeyObject(".....!.,..,.?..|"));
            Encryption.Items.Add(new AESKeyObject("*PlayBNS(c)2014*"));
            Encryption.Items.Add(new AESKeyObject("0509bns&dj@#!yqw"));
            /* Fill XOR Keys */
            Signature.Items.Add(new RSAKeyObject(bnscompression.GetRSAKeyBlob(Convert.FromBase64String("AQAB"), Convert.FromBase64String("sa2gx7Plwlm1gFhlHw4AMYyNWQ/CvYi8zpJTstH9YjmCrZyH9iNoJAHGThc+EBO2zrupipswt08iKWduGVb/F4enLXiSpBjM7wnv4a10tLXp8BRoLogyM9cR0+al7Y18nfbTRvU2wyoAiNUz/asAKTlxzp5TagqhUTywiT8KxeM="), Convert.FromBase64String("x2jNWvgj/B/MX4Kq/BQ5ksHjAhU7ebgf7aPA/jPtnV6y+nLwDkn7DVV83q5UV+u2m45EKrxTOEV7py7rQP7Yzw=="), Convert.FromBase64String("5BoL3To7yzDBcFjO9TfYNX+YFIziyxEvpD4qo6JWgAisjpO6xWYkm6jed8GqEI61k1c1Wkw8ASh0iCQ0mnYerQ=="))));
            // OG Signature Keys
            Signature.Items.Add(new RSAKeyObject(bnscompression.GetRSAKeyBlob(Convert.FromBase64String("AQAB"), Convert.FromBase64String("tBVLclztm4R9xHg42AZAUJb8TpkyGKGxPdvBzGJ9Sy/Yw0ESNHekV4UztGSDoq73tJJuHdxdXylMOzsOOVUQmEYwfuMUQHYp30W6lwAfAFyL86PcI0J7x8DyR4XKvLACGeLsauwEPT7+vzahwF4vbPQGxBW3LFmLPADVFDVUxHU="), Convert.FromBase64String("xbn43YQcrWmz45VS0vVZbLWEvP8RrPxEHk9zmgbCHwxnhnkP/H4TCimSlQi+95jYIeJYi2o+fy0fBFMYIDNIAw=="), Convert.FromBase64String("6SgtKae40mEt4fDE89ToGB8eKnH1ltpjWye1LgHPi2C7JRTrG2hGd5zUEwMUq/zaocaap9osovg5uiKexMxEJw=="))));
            Signature.Items.Add(new RSAKeyObject(bnscompression.GetRSAKeyBlob(Convert.FromBase64String("AQAB"), Convert.FromBase64String("6frEEJqRXEuy/ttKNKxRZZdvqAgeSi0yDwMzMu4lZhtq4/sbojbQH2zkcsEUz6PJ7Ab9Zty2EuBDO1ZJoYN2Y0i1Pvi+avGGJbwTuHuPag352hxHwVPbBXZ//koxlL4J1J9FQKtEWHBCRkDM7UYVBkCQb5I6k9fEtyJejrzdmgk="), Convert.FromBase64String("8mJvLTFUS3w15GT+//Ok7xSZlO2SRtypmhcCrtAFUGDbmjmIT9Wg8Ll353yDGzFxZIVmiMblgdMrRnc1d7pf6Q=="), Convert.FromBase64String("9x93N77SSk7vsZdzuS9eutc+zMKxk5fBYqndgK6gm8+mSfKWBm4CCKVWXNeuhIYtsgSOBeix5nYvjkymVpw1IQ=="))));
            Signature.Items.Add(new RSAKeyObject(bnscompression.GetRSAKeyBlob(Convert.FromBase64String("AQAB"), Convert.FromBase64String("4n/9xPwCpn2/TGXY0bCc23xXKdU9iobCl2RCMWTDgz17uh+Jl8W+Jvci+apyTyXDYdQH8nh2SKkUpAYsQy8bA9v8k+ZbYDytp/DAcHKBfY/1ccknJQrWStbzxQwRXSGsmWmY0vwCW2K7iTkWGQbxo0qRG/L10/qDXQLxf7bmyE8="), Convert.FromBase64String("6fXfeI2dhsT173QVhtOpYLqEQazrsf9opL8cps8j6XH5AzGpRDh0PePoXzhWTZA36nbyEJY0yqDrrBVBEQwRnQ=="), Convert.FromBase64String("99Y0G0QkA62/hFBFmg5fI4vdsesCYGMZw+QwhdSJW87Z5fTZ8r8PamYzNudQPeiJhgQgAVjpFBG7K6Um6JRj2w=="))));
            Signature.Items.Add(new RSAKeyObject(bnscompression.GetRSAKeyBlob(Convert.FromBase64String("AQAB"), Convert.FromBase64String("1RYNfdl1iOQhCuzBWonxWcjm8LGdBTryXzwOe7F1KAMrBpI8unKp2m5bJErJtcQ8jsZd/ZsefdQK13QrQnS4JxT5cVEztrzlG0SPXL2x+ACxv41hajtRbEkUSXWBYBS0pbHZDDbSBvnQCKsVZPrbWv5BMc7QwgP0JPbAhi6WZoE="), Convert.FromBase64String("2mufi1EqQCiFAn5A/imiUfh+qsJ0ME6/iZr9K8c8T5S62mI2PcfzSXbFfbBKqLMZsc6BM3Z5JT9ZeBIta2GbCQ=="), Convert.FromBase64String("+b97FJ/96M2pTH1wDo28ifb4m4MJ4N5deLvq8qqiOdcAdyiRofZG35LGHqNX0ZnaNDY8/dU5mpBmlMUQ6vC1uQ=="))));
            // Fill Compression Level
            Compression.Maximum = 4;
            Compression.Minimum = 0;
            Compression.SmallChange = 1;
            Compression.LargeChange = 1;
            // Default Values
            Encryption.SelectedIndex = 0;
            Signature.SelectedIndex = 0;
            BXML.Checked = false;
            BXMLv3.Checked = false;
            BXMLv4.Checked = true;
        }

        // Virtual Elements for dat reading settings
        public CheckBox Use64Bit = new CheckBox();
        public ComboBox Encryption = new ComboBox();
        public CheckBox UseEncryption = new CheckBox();
        public ComboBox Signature = new ComboBox();
        public CheckBox UseSignature = new CheckBox();
        public TrackBar Compression = new TrackBar();
        public CheckBox UseCompression = new CheckBox();
        public RadioButton BXML = new RadioButton();
        public RadioButton BXMLv3 = new RadioButton();
        public RadioButton BXMLv4 = new RadioButton();
    }
}

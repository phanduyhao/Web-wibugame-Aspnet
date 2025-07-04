﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace accgame.Mahoa
{
    public class MyString
    {
        public static byte[] encryptData(string data)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedBytes;
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
            return hashedBytes;
        }
        public static string md5(string data)
        {
            return BitConverter.ToString(encryptData(data)).Replace("-", "").ToLower();
        }

        public static string vietTat(string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;
            var tenVietTat = string.Concat(data.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                              .Select(w => w[0]));
            return tenVietTat.ToUpper();
        }
    }
}
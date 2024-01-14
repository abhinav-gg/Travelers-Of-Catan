﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileHandler // give credit to person who made this
{
    private string filepath = "";
    public bool IsMade;
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "aiuaeogmk3GJEK834FEJSAK";

    public FileHandler(string filepath, bool useEncryption)
    {
        this.filepath = filepath;
        this.useEncryption = useEncryption;
        IsMade = File.Exists(filepath);
    }

    public void Delete()
    {
        if (IsMade)
        {
            File.Delete(filepath);
            IsMade = false;
        }
    }


    public string Load()
    {

        string dataToLoad = "";
        if (File.Exists(filepath))
        {
            try
            {
                // load the serialized data from the file
                using (FileStream stream = new FileStream(filepath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // optionally decrypt the data
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

            }
            catch (Exception e)
            {
                Debug.LogError($"Error occured when trying to load data {e}");
            }
        }
        return dataToLoad;
    }

    public void Save(string data)
    {

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));

            if (useEncryption)
            {
                data = EncryptDecrypt(data);
            }

            // write the serialized data to the file
            using (FileStream stream = new FileStream(filepath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(data);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + filepath + "\n" + e);
        }
    }

    /// <summary>
    /// Perform XOR encryption/decryption on the data
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public string EncryptDecrypt(string data)
    {
        string key = encryptionCodeWord;
        int dataLen = data.Length;
        int keyLen = key.Length;
        char[] output = new char[dataLen];

        for (int i = 0; i < dataLen; ++i)
        {
            output[i] = (char)(data[i] ^ key[i % keyLen]);
        }

        return new string(output);
    }

}
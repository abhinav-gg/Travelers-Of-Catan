﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileHandler // Credit to https://www.youtube.com/watch?v=KZft1p8t2lQ for the tutorial on how to do 
{
    private string filepath = "";
    public bool IsMade;
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "SomeRandomStringKeyToConvertIntoBinaryForXOR->aiuaeogmk3GJEK834FEJSAK->";
    // practically speaking, this is not very easily crackable, but it's not the most secure either as it uses the same key for every file

    public FileHandler(string filepath)
    {
        this.filepath = filepath;
        encryptionCodeWord += filepath; // add the filepath to the encryption key to make it more unique
        string suffix = FindSuffix();
        if (suffix == "")
        {
            IsMade = false;
            this.filepath += ".bin";
            useEncryption = true;
        }
        else
        {
            IsMade = true;
            useEncryption = suffix == ".bin";
            this.filepath += suffix;
        }
    }


    string FindSuffix()
    {
        if (File.Exists(filepath + ".json"))
        {
            return ".json";
        }
        else if (File.Exists(filepath + ".bin"))
        {
            return ".bin";
        }
        else
        {
            return "";
        }
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
    public string EncryptDecrypt(string data) // Credit to https://stackoverflow.com/questions/2532668/help-me-with-xor-encryption for the XOR encryption
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
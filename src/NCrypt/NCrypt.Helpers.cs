﻿// Copyright (c) to owners found in https://github.com/AArnott/pinvoke/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace PInvoke
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    /// <content>
    /// Methods and nested types that are not strictly P/Invokes but provide
    /// a slightly higher level of functionality to ease calling into native code.
    /// </content>
    public static partial class NCrypt
    {
        /// <summary>
        /// Loads and initializes a CNG key storage provider.
        /// </summary>
        /// <param name="providerName">
        /// A pointer to a null-terminated Unicode string that identifies the key storage provider to load. This is the registered alias of the key storage provider. This parameter is optional and can be NULL. If this parameter is NULL, the default key storage provider is loaded. The <see cref="KeyStorageProviders"/> class identifies the built-in key storage providers.
        /// </param>
        /// <param name="flags">Flags that modify the behavior of the function.</param>
        /// <returns>
        /// A <see cref="SafeProviderHandle"/> variable that receives the provider handle. When you have finished using this handle, dispose of it.
        /// </returns>
        public static SafeProviderHandle NCryptOpenStorageProvider(
            string providerName,
            NCryptOpenStorageProviderFlags flags = NCryptOpenStorageProviderFlags.None)
        {
            SafeProviderHandle handle;
            NCryptOpenStorageProvider(out handle, providerName, flags).ThrowOnError();
            return handle;
        }

        /// <summary>
        /// Creates a new key and stores it in the specified key storage provider. After you create a key by using this function, you can use the NCryptSetProperty function to set its properties; however, the key cannot be used until the NCryptFinalizeKey function is called.
        /// </summary>
        /// <param name="provider">
        /// The handle of the key storage provider to create the key in. This handle is obtained by using the <see cref="NCryptOpenStorageProvider(string, NCryptOpenStorageProviderFlags)"/> function.
        /// </param>
        /// <param name="algorithmId">
        /// A null-terminated Unicode string that contains the identifier of the cryptographic algorithm to create the key. This can be one of the standard CNG Algorithm Identifiers defined in <see cref="BCrypt.AlgorithmIdentifiers"/> or the identifier for another registered algorithm.
        /// </param>
        /// <param name="keyName">
        /// A pointer to a null-terminated Unicode string that contains the name of the key. If this parameter is NULL, this function will create an ephemeral key that is not persisted.
        /// </param>
        /// <param name="legacyKeySpec">
        /// A legacy identifier that specifies the type of key.
        /// </param>
        /// <param name="flags">A set of flags that modify the behavior of this function.</param>
        /// <returns>
        /// The address of an <see cref="SafeKeyHandle"/> variable that receives the handle of the key. When you have finished using this handle, release it by disposing it.
        /// </returns>
        public static SafeKeyHandle NCryptCreatePersistedKey(
            SafeProviderHandle provider,
            string algorithmId,
            string keyName = null,
            LegacyKeySpec legacyKeySpec = LegacyKeySpec.None,
            NCryptCreatePersistedKeyFlags flags = NCryptCreatePersistedKeyFlags.None)
        {
            SafeKeyHandle result;
            NCryptCreatePersistedKey(
                provider,
                out result,
                algorithmId,
                keyName,
                legacyKeySpec,
                flags).ThrowOnError();
            return result;
        }

        /// <summary>
        /// The NCryptExportKey function exports a CNG key to a memory BLOB.
        /// </summary>
        /// <param name="key">A handle of the key to export.</param>
        /// <param name="exportKey">A handle to a cryptographic key of the destination user. The key data within the exported key BLOB is encrypted by using this key. This ensures that only the destination user is able to make use of the key BLOB.</param>
        /// <param name="blobType">A null-terminated Unicode string that contains an identifier that specifies the type of BLOB to export. This can be one of the values defined by the <see cref="BCrypt.AsymmetricKeyBlobTypes"/> or <see cref="BCrypt.SymmetricKeyBlobTypes"/> classes.</param>
        /// <param name="parameterList">The address of an NCryptBufferDesc structure that receives parameter information for the key. This parameter can be NULL if this information is not needed.</param>
        /// <param name="flags">Flags that modify function behavior. This can be zero or a combination of one or more of the following values. The set of valid flags is specific to each key storage provider.</param>
        /// <returns>Returns the exported key.</returns>
        /// <exception cref="SecurityStatusException">Thrown if an error code is returned from the native function.</exception>
        /// <remarks>
        /// A service must not call this function from its StartService Function. If a service calls this function from its StartService function, a deadlock can occur, and the service may stop responding.
        /// </remarks>
        public static unsafe ArraySegment<byte> NCryptExportKey(
            SafeKeyHandle key,
            SafeKeyHandle exportKey,
            string blobType,
            NCryptBufferDesc* parameterList,
            NCryptExportKeyFlags flags = NCryptExportKeyFlags.None)
        {
            int pcbResult;
            NCryptExportKey(key, exportKey, blobType, parameterList, null, 0, out pcbResult, flags).ThrowOnError();
            byte[] result = new byte[pcbResult];
            NCryptExportKey(key, exportKey, blobType, parameterList, result, result.Length, out pcbResult, flags).ThrowOnError();
            return new ArraySegment<byte>(result, 0, pcbResult);
        }

        /// <summary>
        /// Retrieves the value of a named property for a key storage object.
        /// </summary>
        /// <param name="hObject">
        /// The handle of the object to get the property for. This can be a provider handle (<see cref="SafeProviderHandle"/>) or a key handle (<see cref="SafeKeyHandle"/>).
        /// </param>
        /// <param name="propertyName">
        /// A pointer to a null-terminated Unicode string that contains the name of the property to retrieve. This can be one of the predefined <see cref="KeyStoragePropertyIdentifiers"/> or a custom property identifier.
        /// </param>
        /// <param name="flags">Flags that modify function behavior.</param>
        /// <returns>The property value.</returns>
        public static byte[] NCryptGetProperty(SafeHandle hObject, string propertyName, NCryptGetPropertyFlags flags = NCryptGetPropertyFlags.None)
        {
            int requiredSize;
            NCryptGetProperty(hObject, propertyName, null, 0, out requiredSize, flags).ThrowOnError();
            byte[] result = new byte[requiredSize];
            NCryptGetProperty(hObject, propertyName, result, result.Length, out requiredSize, flags).ThrowOnError();
            return result;
        }

        /// <summary>
        /// Retrieves the value of a named property for a key storage object.
        /// </summary>
        /// <typeparam name="T">The type of struct to return the property value as.</typeparam>
        /// <param name="hObject">
        /// The handle of the object to get the property for. This can be a provider handle (<see cref="SafeProviderHandle"/>) or a key handle (<see cref="SafeKeyHandle"/>).
        /// </param>
        /// <param name="propertyName">
        /// A pointer to a null-terminated Unicode string that contains the name of the property to retrieve. This can be one of the predefined <see cref="KeyStoragePropertyIdentifiers"/> or a custom property identifier.
        /// </param>
        /// <param name="flags">Flags that modify function behavior.</param>
        /// <returns>The property value.</returns>
        public static T NCryptGetProperty<T>(SafeHandle hObject, string propertyName, NCryptGetPropertyFlags flags = NCryptGetPropertyFlags.None)
            where T : struct
        {
            byte[] value = NCryptGetProperty(hObject, propertyName, flags);
            unsafe
            {
                fixed (byte* pValue = value)
                {
                    return (T)Marshal.PtrToStructure(new IntPtr(pValue), typeof(T));
                }
            }
        }

        /// <summary>
        /// Sets the value of a named property for a CNG object.
        /// </summary>
        /// <param name="hObject">A handle that represents the CNG object to set the property value for.</param>
        /// <param name="propertyName">
        /// The name of the property to set. This can be one of the predefined <see cref="KeyStoragePropertyIdentifiers"/> or a custom property identifier.
        /// </param>
        /// <param name="propertyValue">The new property value.</param>
        public static void NCryptSetProperty(SafeHandle hObject, string propertyName, string propertyValue)
        {
            var error = NCryptSetProperty(
                hObject,
                propertyName,
                propertyValue,
                propertyValue != null ? (propertyValue.Length + 1) * sizeof(char) : 0,
                0);
            error.ThrowOnError();
        }

        /// <summary>
        /// Sets the value of a named property for a CNG object.
        /// </summary>
        /// <typeparam name="T">The type of value being set.</typeparam>
        /// <param name="hObject">A handle that represents the CNG object to set the property value for.</param>
        /// <param name="propertyName">
        /// The name of the property to set. This can be one of the predefined <see cref="KeyStoragePropertyIdentifiers"/> or a custom property identifier.
        /// </param>
        /// <param name="propertyValue">The new property value.</param>
        /// <param name="flags">Flags to pass to <see cref="NCryptSetProperty(SafeHandle, string, byte*, int, NCryptSetPropertyFlags)"/></param>
        public static unsafe void NCryptSetProperty<T>(SafeHandle hObject, string propertyName, T propertyValue, NCryptSetPropertyFlags flags = NCryptSetPropertyFlags.None)
        {
            int bufferSize = Marshal.SizeOf(propertyValue);
            fixed (byte* valueBuffer = new byte[bufferSize])
            {
                Marshal.StructureToPtr(propertyValue, new IntPtr(valueBuffer), false);
                try
                {
                    NCryptSetProperty(hObject, propertyName, valueBuffer, bufferSize, flags).ThrowOnError();
                }
                finally
                {
                    Marshal.DestroyStructure(new IntPtr(valueBuffer), typeof(T));
                }
            }
        }

        /// <summary>
        /// Encrypts a block of data.
        /// </summary>
        /// <param name="key">
        /// The handle of the key to use to encrypt the data.
        /// </param>
        /// <param name="plaintext">
        /// The address of a buffer that contains the plaintext to be encrypted. The cbInput parameter contains the size of the plaintext to encrypt.
        /// </param>
        /// <param name="paddingInfo">
        /// A pointer to a structure that contains padding information. This parameter is only used with asymmetric keys and authenticated encryption modes. If an authenticated encryption mode is used, this parameter must point to a BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO structure. If asymmetric keys are used, the type of structure this parameter points to is determined by the value of the dwFlags parameter. Otherwise, the parameter must be set to NULL.
        /// </param>
        /// <param name="flags">
        /// A set of flags that modify the behavior of this function. The allowed set of flags depends on the type of key specified by the hKey parameter.
        /// </param>
        /// <returns>Returns the ciphertext.</returns>
        public static unsafe ArraySegment<byte> NCryptEncrypt(SafeKeyHandle key, byte[] plaintext, void* paddingInfo = null, NCryptEncryptFlags flags = NCryptEncryptFlags.None)
        {
            fixed (byte* pPlaintext = plaintext)
            {
                int pcbResult;
                NCryptEncrypt(key, pPlaintext, plaintext.Length, paddingInfo, null, 0, out pcbResult, flags).ThrowOnError();
                byte[] ciphertext = new byte[pcbResult];
                fixed (byte* pCiphertext = ciphertext)
                {
                    NCryptEncrypt(key, pPlaintext, plaintext.Length, paddingInfo, pCiphertext, pcbResult, out pcbResult, flags).ThrowOnError();
                    return new ArraySegment<byte>(ciphertext, 0, pcbResult);
                }
            }
        }

        /// <summary>
        /// Decrypts a block of data.
        /// </summary>
        /// <param name="key">
        /// The handle of the key to use to decrypt the data.
        /// </param>
        /// <param name="ciphertext">
        /// The address of a buffer that contains the ciphertext to be decrypted. The <paramref name="ciphertext"/> parameter contains the size of the ciphertext to decrypt. For more information, see Remarks.
        /// </param>
        /// <param name="paddingInfo">
        /// A pointer to a structure that contains padding information. This parameter is only used with asymmetric keys and authenticated encryption modes. If an authenticated encryption mode is used, this parameter must point to a BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO structure. If asymmetric keys are used, the type of structure this parameter points to is determined by the value of the <paramref name="flags"/> parameter. Otherwise, the parameter must be set to NULL.
        /// </param>
        /// <param name="flags">
        /// A set of flags that modify the behavior of this function. The allowed set of flags depends on the type of key specified by the <paramref name="key"/> parameter.
        /// </param>
        /// <returns>Returns the plaintext.</returns>
        public static unsafe ArraySegment<byte> NCryptDecrypt(SafeKeyHandle key, byte[] ciphertext, void* paddingInfo = null, NCryptEncryptFlags flags = NCryptEncryptFlags.None)
        {
            fixed (byte* pCiphertext = ciphertext)
            {
                int pcbResult;
                NCryptDecrypt(key, pCiphertext, ciphertext.Length, paddingInfo, null, 0, out pcbResult, flags).ThrowOnError();
                byte[] plaintext = new byte[pcbResult];
                fixed (byte* pPlaintext = plaintext)
                {
                    NCryptDecrypt(key, pCiphertext, ciphertext.Length, paddingInfo, pPlaintext, pcbResult, out pcbResult, flags).ThrowOnError();
                    return new ArraySegment<byte>(plaintext, 0, pcbResult);
                }
            }
        }
    }
}

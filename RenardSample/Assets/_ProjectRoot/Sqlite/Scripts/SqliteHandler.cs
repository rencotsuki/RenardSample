﻿/*
 * 参考：Busta117/SQLiteUnityKit
 * https://github.com/Busta117/SQLiteUnityKit
 * 
 * 参考：UnityでSQLiteを扱う方法
 * https://qiita.com/hiroyuki7/items/5335e391c9ed397aee50
 * 
 * 参考：UnityでSQLiteをAndroid(64bit対応)向けに導入する
 * https://qiita.com/tetr4lab/items/729008c94daaff82833e
 */
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SignageHADO.Sqlite
{
    public class SqliteException : Exception
    {
        public SqliteException(string message) : base(message) {}
    }

    public class SqliteHandler
    {
        public static bool IsDebugLog => false;

        public const string FileExtension = "db";

        public bool CanExQuery { get; protected set; } = false;

        #region 戻り値
        const int sqlite_return_OK      = 0;
        const int sqlite_return_ROW     = 100;
        const int sqlite_return_DONE    = 101;
        const int sqlite_return_INTEGER = 1;
        const int sqlite_return_FLOAT   = 2;
        const int sqlite_return_TEXT    = 3;
        const int sqlite_return_BLOB    = 4;
        const int sqlite_return_NULL    = 5;
        #endregion

#if (UNITY_IOS || UNITY_EDITOR_OSX) && !UNITY_EDITOR_WIN
        private const string _sqlite_DLLName = "libsqlite3";
#else
        private const string _sqlite_DLLName = "libsqliteX";
#endif

        [DllImport(_sqlite_DLLName, EntryPoint = "sqlite3_open")]
        private static extern int sqlite3_open(string filename, out IntPtr db);

        [DllImport(_sqlite_DLLName, EntryPoint = "sqlite3_close")]
        private static extern int sqlite3_close(IntPtr db);

        [DllImport(_sqlite_DLLName, EntryPoint = "sqlite3_prepare_v2")]
        private static extern int sqlite3_prepare_v2(IntPtr db, string zSql, int nByte, out IntPtr ppStmpt, IntPtr pzTail);

        [DllImport(_sqlite_DLLName, EntryPoint = "sqlite3_step")]
        private static extern int sqlite3_step(IntPtr stmHandle);

        [DllImport(_sqlite_DLLName, EntryPoint = "sqlite3_finalize")]
        private static extern int sqlite3_finalize(IntPtr stmHandle);

        [DllImport(_sqlite_DLLName, EntryPoint = "sqlite3_errmsg")]
        private static extern IntPtr sqlite3_errmsg(IntPtr db);

        [DllImport(_sqlite_DLLName, EntryPoint = "sqlite3_column_count")]
        private static extern int sqlite3_column_count(IntPtr stmHandle);

        [DllImport(_sqlite_DLLName, EntryPoint = "sqlite3_column_name")]
        private static extern IntPtr sqlite3_column_name(IntPtr stmHandle, int iCol);

        [DllImport(_sqlite_DLLName, EntryPoint = "sqlite3_column_type")]
        private static extern int sqlite3_column_type(IntPtr stmHandle, int iCol);

        [DllImport(_sqlite_DLLName, EntryPoint = "sqlite3_column_int")]
        private static extern int sqlite3_column_int(IntPtr stmHandle, int iCol);

        [DllImport(_sqlite_DLLName, EntryPoint = "sqlite3_column_text")]
        private static extern IntPtr sqlite3_column_text(IntPtr stmHandle, int iCol);

        [DllImport(_sqlite_DLLName, EntryPoint = "sqlite3_column_double")]
        private static extern double sqlite3_column_double(IntPtr stmHandle, int iCol);

        [DllImport(_sqlite_DLLName, EntryPoint = "sqlite3_column_blob")]
        private static extern IntPtr sqlite3_column_blob(IntPtr stmHandle, int iCol);

        [DllImport(_sqlite_DLLName, EntryPoint = "sqlite3_column_bytes")]
        private static extern int sqlite3_column_bytes(IntPtr stmHandle, int iCol);

        private IntPtr _connection;

        private bool _isConnectionOpen { get; set; }

        private string _dbFileName = string.Empty;
        private string _dbDirectoryPath = string.Empty;
        private string dbPath => $"{_dbDirectoryPath}/{_dbFileName}.{FileExtension}";

        #region Public Methods

        /// <summary>
        /// SQLite本体
        /// </summary>
        /// <param name="dbDirectoryPath">DBディレクトリパス</param>
        /// <param name="dbFileName">DBファイル名</param>
        public SqliteHandler(string dbDirectoryPath, string dbFileName)
        {
            _dbFileName = dbFileName;
            _dbDirectoryPath = dbDirectoryPath;
            CanExQuery = File.Exists(dbPath);
            _isConnectionOpen = false;
        }

        private void Open()
        {
            if (string.IsNullOrEmpty(_dbFileName))
            {
                if (IsDebugLog)
                    Debug.Log("ERROR: DB fileName is Null or Empty");
                return;
            }

            if (_isConnectionOpen)
            {
                throw new SqliteException("There is already an open connection");
            }

            if (!File.Exists(dbPath))
            {
                throw new SqliteException($"NoExists database path: {dbPath}");
            }

            var rtn = sqlite3_open(dbPath, out _connection);
            if (rtn != sqlite_return_OK)
            {
                throw new SqliteException($"Could not open database: {dbPath}");
            }

            _isConnectionOpen = true;
        }

        public void Close()
        {
            if (_isConnectionOpen)
            {
                sqlite3_close(_connection);
            }

            _isConnectionOpen = false;
        }

        /// <summary>
        /// クエリ操作[書込み系]
        /// </summary>
        public bool ExecuteNonQuery(string query)
        {
            if (!CanExQuery)
            {
                if (IsDebugLog)
                    Debug.Log($"ERROR: Can't execute the query, verify DB origin file.");
                return false;
            }

            Open();

            if (!_isConnectionOpen)
            {
                throw new SqliteException("SQLite database is not open.");
            }

            IntPtr stmHandle = Prepare(query);

            if (sqlite3_step(stmHandle) != sqlite_return_DONE)
            {
                throw new SqliteException("Could not execute SQL statement.");
            }

            Finalize(stmHandle);
            Close();

            return true;
        }

        /// <summary>
        /// クエリ操作[呼出し系]
        /// </summary>
        public DataTable ExecuteQuery(string query)
        {
            if (!CanExQuery)
            {
                if (IsDebugLog)
                    Debug.Log("ERROR: Can't execute the query, verify DB origin file.");
                return null;
            }

            Open();

            if (!_isConnectionOpen)
            {
                throw new SqliteException("SQLite database is not open.");
            }

            IntPtr stmHandle = Prepare(query);

            int columnCount = sqlite3_column_count(stmHandle);

            var dataTable = new DataTable();
            for (int i = 0; i < columnCount; i++)
            {
                string columnName = Marshal.PtrToStringAnsi(sqlite3_column_name(stmHandle, i));
                dataTable.Columns.Add(columnName);
            }

            while (sqlite3_step(stmHandle) == sqlite_return_ROW)
            {
                object[] row = new object[columnCount];
                for (int i = 0; i < columnCount; i++)
                {
                    switch (sqlite3_column_type(stmHandle, i))
                    {
                        case sqlite_return_INTEGER:
                            row[i] = sqlite3_column_int(stmHandle, i);
                            break;

                        case sqlite_return_TEXT:
                            IntPtr text = sqlite3_column_text(stmHandle, i);
                            row[i] = Marshal.PtrToStringAnsi(text);
                            break;

                        case sqlite_return_FLOAT:
                            row[i] = sqlite3_column_double(stmHandle, i);
                            break;

                        case sqlite_return_BLOB:
                            IntPtr blob = sqlite3_column_blob(stmHandle, i);
                            int size = sqlite3_column_bytes(stmHandle, i);
                            byte[] data = new byte[size];
                            Marshal.Copy(blob, data, 0, size);
                            row[i] = data;
                            break;

                        case sqlite_return_NULL:
                            row[i] = null;
                            break;
                    }
                }

                dataTable.AddRow(row);
            }

            Finalize(stmHandle);
            Close();

            return dataTable;
        }

        // クエリ操作[書込み系]
        protected bool ExecuteNonQuery(string[] querys, bool isViewDialog = false, Action<int, int> onUpdate = null)
        {
            if (!CanExQuery)
            {
                if (IsDebugLog)
                    Debug.Log("ERROR: Can't execute the query, verify DB origin file");
                return false;
            }

            if (querys == null || querys.Length <= 0)
            {
                if (IsDebugLog)
                    Debug.Log("ERROR: There is not one query");
                return false;
            }

            Open();

            if (!_isConnectionOpen)
            {
                throw new SqliteException("SQLite database is not open.");
            }

            if (isViewDialog)
                Debug.Log($"SQLite - Open DB: {_dbFileName}");

            for (int i = 0; i < querys.Length; i++)
            {
                if (onUpdate != null)
                    onUpdate(i, querys.Length);

                if (isViewDialog)
                    Debug.Log($"SQLite - Write DB: {i}/{querys.Length}");

                if (string.IsNullOrEmpty(querys[i]))
                    continue;

                IntPtr stmHandle = Prepare(querys[i]);

                if (sqlite3_step(stmHandle) != sqlite_return_DONE)
                {
                    throw new SqliteException("Could not execute SQL statement.");
                }

                Finalize(stmHandle);
            }

            if (isViewDialog)
                Debug.Log($"SQLite - Close DB: {_dbFileName}");

            Close();

            return true;
        }

        /// <summary>
        /// スクリプト操作
        /// </summary>
        public bool ExecuteScript(string script, bool isViewDialog = false, Action<int, int> onUpdate = null)
        {
            var statements = script.Split(';');
            var isSuccess = false;
            if (statements != null && statements.Length > 0)
            {
                try
                {
                    isSuccess = ExecuteNonQuery(statements, isViewDialog, onUpdate);
                }
                catch (SqliteException ex)
                {
                    if (IsDebugLog)
                        Debug.Log("ExecuteScriptForEditor: " + ex.Message);

                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        #endregion

        #region Private Methods

        private IntPtr Prepare(string query)
        {
            IntPtr stmHandle;
            var byteCount = System.Text.Encoding.UTF8.GetByteCount(query);

            if (sqlite3_prepare_v2(_connection, query, byteCount, out stmHandle, IntPtr.Zero) != sqlite_return_OK)
            {
                IntPtr errorMsg = sqlite3_errmsg(_connection);
                throw new SqliteException(Marshal.PtrToStringAnsi(errorMsg));
            }

            return stmHandle;
        }

        private void Finalize(IntPtr stmHandle)
        {
            if (sqlite3_finalize(stmHandle) != sqlite_return_OK)
            {
                throw new SqliteException("Could not finalize SQL statement.");
            }
        }

        #endregion
    }
}

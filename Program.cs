using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NLog;

using Transferizer.Configuration;
using Transferizer.Model;

namespace Transferizer
{
    class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                Logger.Info("Inicio de la aplicación");
                TransferizerManager manager = new TransferizerManager();
                manager.ReadConfiguration();

                Parallel.ForEach(manager.Transfers, transfer =>
                {
                    try
                    {
                        if (manager.Exists(transfer.From) && manager.Exists(transfer.To))
                            manager.MoveFiles(transfer);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Error");
                    }
                });

                Logger.Info("Finaliza la ejecucióno de la aplicación");
            }
            catch(Exception ex)
            {
                Logger.Error(ex, "Goodbye cruel world");
            }

        }
    }

    public class TransferizerManager
    {
        private IEnumerable<Transfer> _transfers;

        public IEnumerable<Transfer> Transfers
        {
            get
            {
                if (_transfers != null)
                    return _transfers;
                else
                    return new List<Transfer>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReadConfiguration()
        {
            System.Configuration.Configuration config =
                ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None) as System.Configuration.Configuration;

            TransferSection transferSection =
                config.GetSection("TransfersSection") as TransferSection;

            var transfers = new List<Transfer>();
            foreach (TransferElement element in transferSection.Transfers)
            {
                Transfer transfer = GetTransfer(element);
                transfers.Add(transfer);
            }

            _transfers = transfers;
        }

        public void MoveFiles(Transfer transfer)
        {
            var files = GetFiles(transfer);
            Parallel.ForEach(files, file => System.IO.File.Copy(file.Name, transfer.To + '\\' + file));               
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        Transfer GetTransfer(TransferElement element)
        {
            Transfer transfer = new Transfer();
            transfer.From = element.From;
            transfer.IncludeSubFolders = element.IncludeSubFolders;
            transfer.SearchPattern = element.SearchPattern;
            transfer.To = element.To;

            return transfer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transfer"></param>
        /// <returns></returns>
        private IEnumerable<FileInfo> GetFiles(Transfer transfer)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(transfer.From);
            IEnumerable<FileInfo> files;

            if (!string.IsNullOrEmpty(transfer.SearchPattern))
            {
                Regex regex = new Regex(transfer.SearchPattern);
                files = sourceDirectory.GetFiles()
                                        .Where(file => regex.IsMatch(file.Name))
                                        .ToList();
            }
            else
            {
                files = sourceDirectory.GetFiles().ToList();
            }

            return files;
        }
    }
}

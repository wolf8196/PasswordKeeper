using System;
using System.Threading.Tasks;
using PasswordKeeper.Services.Entities;

namespace PasswordKeeper.Apps.Wpf.Abstractions.Services
{
    public interface IFileProcessor : ISubscribable
    {
        event Action ProcessingFinished;

        Account SelectedAccount { get; set; }

        Task ProcessFiles(string[] obj);
    }
}
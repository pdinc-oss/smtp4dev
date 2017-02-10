using System;

namespace Rnwood.Smtp4dev.Model
{
    public interface ISmtp4DevEngine
    {
        bool IsRunning { get; }

        Exception ServerError { get; }

        event EventHandler StateChanged;
    }
}
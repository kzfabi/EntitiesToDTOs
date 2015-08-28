/* EntitiesToDTOs. Copyright (c) 2011. Fabian Fernandez.
 * http://entitiestodtos.codeplex.com
 * Licensed by Common Development and Distribution License (CDDL).
 * http://entitiestodtos.codeplex.com/license
 * Fabian Fernandez. 
 * http://www.linkedin.com/in/fabianfernandezb/en
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitiesToDTOs.Events
{
    /// <summary>
    /// Arguments of the GeneratorManager OnProgress event.
    /// </summary>
    internal class GeneratorOnProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Indicates the current progress.
        /// </summary>
        public int Progress { get; private set; }

        /// <summary>
        /// Status message.
        /// </summary>
        public string StatusMessage { get; private set; }


        public GeneratorOnProgressEventArgs(int progress, string statusMessage)
        {
            this.Progress = progress;
            this.StatusMessage = statusMessage;
        }
    }

    /// <summary>
    /// Arguments of the GeneratorManager OnComplete event.
    /// </summary>
    internal class GeneratorOnCompleteEventArgs : EventArgs { }

    /// <summary>
    /// Arguments of the GeneratorManager OnCancel event.
    /// </summary>
    internal class GeneratorOnCancelEventArgs : EventArgs { }

    /// <summary>
    /// Arguments of the GeneratorManager OnException event.
    /// </summary>
    internal class GeneratorOnExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Reference to the Exception raised.
        /// </summary>
        public Exception Exception { get; private set; }


        public GeneratorOnExceptionEventArgs(Exception exception)
        {
            this.Exception = exception;
        }
    }
}
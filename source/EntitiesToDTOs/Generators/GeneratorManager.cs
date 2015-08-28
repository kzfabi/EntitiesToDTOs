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
using EntitiesToDTOs.Events;
using EntitiesToDTOs.Properties;
using System.ComponentModel;
using System.Threading;
using EntitiesToDTOs.Helpers;
using EntitiesToDTOs.Generators.Parameters;
using EntitiesToDTOs.Domain;

namespace EntitiesToDTOs.Generators
{
    /// <summary>
    /// Manages the generation process.
    /// </summary>
    internal class GeneratorManager
    {
        #region Members

        /// <summary>
        /// BackgroundWorker used to execute the generation process in a background thread.
        /// </summary>
        private static BackgroundWorker _worker = null;

        /// <summary>
        /// AutoResetEvent used to wait for background thread cancellation.
        /// </summary>
        private static AutoResetEvent _resetEvent = null;

        #endregion Members

        #region Events

        /// <summary>
        /// Event raised when there is a progress in the generation process.
        /// </summary>
        public static event EventHandler<GeneratorOnProgressEventArgs> OnProgress;

        /// <summary>
        /// Event raised when the generation process finishes.
        /// </summary>
        public static event EventHandler<GeneratorOnCompleteEventArgs> OnComplete;

        /// <summary>
        /// Event raised when the generation process is cancelled.
        /// </summary>
        public static event EventHandler<GeneratorOnCancelEventArgs> OnCancel;

        /// <summary>
        /// Event raised when there is an Exception in the generation process.
        /// </summary>
        public static event EventHandler<GeneratorOnExceptionEventArgs> OnException;

        #endregion Events

        #region Methods

        /// <summary>
        /// Clears all event handlers
        /// </summary>
        public static void ClearEventHandlers()
        {
            GeneratorManager.OnCancel = null;
            GeneratorManager.OnComplete = null;
            GeneratorManager.OnException = null;
            GeneratorManager.OnProgress = null;
        }

        /// <summary>
        /// Raises a GeneratorManager event.
        /// </summary>
        /// <typeparam name="T">Type of arguments to send to the event. The event to raise will be determined by the args type.</typeparam>
        /// <param name="eventArgs">Arguments to pass to the event to raise.</param>
        public static void RaiseEvent<T>(T eventArgs) where T : EventArgs
        {
            object sender = null;

            if (eventArgs is GeneratorOnProgressEventArgs)
            {
                GeneratorManager.OnProgress(sender, (eventArgs as GeneratorOnProgressEventArgs));
            }
            else if (eventArgs is GeneratorOnCompleteEventArgs)
            {
                GeneratorManager.OnComplete(sender, (eventArgs as GeneratorOnCompleteEventArgs));
            }
            else if (eventArgs is GeneratorOnCancelEventArgs)
            {
                GeneratorManager.OnCancel(sender, (eventArgs as GeneratorOnCancelEventArgs));
            }
            else if (eventArgs is GeneratorOnExceptionEventArgs)
            {
                GeneratorManager.OnException(sender, (eventArgs as GeneratorOnExceptionEventArgs));
            }
            else
            {
                throw new ArgumentException(Resources.Error_InvalidEventArgsType);
            }
        }

        /// <summary>
        /// Cancels the generation process.
        /// </summary>
        public static void CancelGeneration()
        {
            if (_worker != null)
            {
                _worker.CancelAsync();

                _resetEvent.WaitOne();

                _worker = null;
                _resetEvent = null;

                // Raise OnCancel event
                GeneratorManager.RaiseEvent<GeneratorOnCancelEventArgs>(new GeneratorOnCancelEventArgs());
            }
        }

        /// <summary>
        /// Checks if there is a worker cancellation pending.
        /// </summary>
        /// <returns></returns>
        public static bool CheckCancellationPending()
        {
            if (_worker != null && _worker.CancellationPending)
            {
                // Wait for worker signal
                _resetEvent.Set();

                // Delete Template Class File
                TemplateClass.Delete();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Starts the generation process.
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public static void Generate(GeneratorManagerParams parameters)
        {
            // Process in a background thread
            _worker = new BackgroundWorker();

            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;

            _worker.DoWork += new DoWorkEventHandler(GeneratorManager.Generate_DoWork);
            _worker.ProgressChanged += new ProgressChangedEventHandler(GeneratorManager.Generate_ProgressChanged);

            _resetEvent = new AutoResetEvent(false);

            _worker.RunWorkerAsync(parameters);
        }

        /// <summary>
        /// Executed when the BackgroundWorker decides to execute.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        static void Generate_DoWork(object sender, DoWorkEventArgs args)
        {
            var worker = (BackgroundWorker)sender;
            var parameters = (GeneratorManagerParams)args.Argument;

            try
            {
                // Generate DTOs
                List<DTOEntity> entitiesDTOs = 
                    DTOGenerator.GenerateDTOs(parameters.DTOsParams, worker);

                if (parameters.GenerateAssemblers)
                {
                    // Check Cancellation Pending
                    if (GeneratorManager.CheckCancellationPending()) return;

                    // Set generated DTOs
                    parameters.AssemblersParams.EntitiesDTOs = entitiesDTOs;

                    // Generate Assemblers
                    AssemblerGenerator.GenerateAssemblers(parameters.AssemblersParams, worker);
                }

                // Report Progress
                worker.ReportProgress(100, new GeneratorOnCompleteEventArgs());
            }
            catch (Exception ex)
            {
                worker.ReportProgress(0, ex);
            }
        }

        /// <summary>
        /// Executed when there is progress reported from BackgroundWorker Logic.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Generate_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var worker = (BackgroundWorker)sender;

            //if (worker.CancellationPending) { } else 
            if (e.UserState is Exception)
            {
                var ex = (Exception)e.UserState;

                // Delete Template Class File
                TemplateClass.Delete();

                // Log Error
                LogManager.LogError(ex);

                // Raise OnException event
                GeneratorManager.RaiseEvent<GeneratorOnExceptionEventArgs>(new GeneratorOnExceptionEventArgs(ex));
            }
            else if (e.UserState is GeneratorOnProgressEventArgs)
            {
                var eventArgs = (GeneratorOnProgressEventArgs)e.UserState;

                // Raise OnProgress event
                GeneratorManager.RaiseEvent<GeneratorOnProgressEventArgs>(eventArgs);
            }
            else if (e.UserState is GeneratorOnCompleteEventArgs)
            {
                var eventArgs = (GeneratorOnCompleteEventArgs)e.UserState;

                // Raise OnComplete event
                GeneratorManager.RaiseEvent<GeneratorOnCompleteEventArgs>(eventArgs);
            }
        }

        #endregion Methods
    }
}
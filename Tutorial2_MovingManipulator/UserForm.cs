using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;


using IpcContractClientInterface;
using AppLog = IpcUtil.Logging;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace Tutorial2_MovingManipulator
{
	public partial class UserForm : Form
	{
		/// <summary>Are we in design mode</summary>
		protected bool mDesignMode { get; private set; }

		#region Standard IPC Variables

		/// <summary>This ensures consistent read and write culture</summary>
		private NumberFormatInfo mNFI = new CultureInfo("en-GB", true).NumberFormat; // Force UN English culture

		/// <summary>Collection of all IPC channels, this object always exists.</summary>
		private Channels mChannels = new Channels();

		#endregion Standard IPC Variables

        #region Application Variables

        /// <summary> Status of the application </summary>
        private Channels.EConnectionState mApplicationState;

        /// <summary> Define the rotate axis (constant) </summary>
        const IpcContract.Manipulator.EAxisName mRotateAxis = IpcContract.Manipulator.EAxisName.Rotate;

        /// <summary> Go signal sent flag </summary>
        private bool mManipulatorGoSignalSent = false;
        /// <summary> Go complete flag </summary>
        private bool mManipulatorGoComplete = false;

        /// <summary> Rotate amount </summary>
        private decimal mRotateAmount = 10;

        /// <summary> Manipulator thread variable </summary>
        private Thread mManipulatorThread = null;

        #endregion Application Variables

        public UserForm()
		{
			try
			{
				mDesignMode = (LicenseManager.CurrentContext.UsageMode == LicenseUsageMode.Designtime);
				InitializeComponent();
				if (!mDesignMode)
				{
					// Tell normal logging who the parent window is.
					AppLog.SetParentWindow = this;
					AppLog.TraceInfo = true;
					AppLog.TraceDebug = true;

					mChannels = new Channels();
					// Enable the channels that will be controlled by this application.
					// For the generic IPC client this is all of them!
					// This just sets flags, it does not actually open the channels.
					mChannels.AccessApplication = true;
					mChannels.AccessXray = false;
					mChannels.AccessManipulator = true;
					mChannels.AccessImageProcessing = false;
                    mChannels.AccessInspection = false;
					mChannels.AccessInspection2D = false;
					mChannels.AccessCT3DScan = false;
					mChannels.AccessCT2DScan = false;
				}
			}
			catch (Exception ex) { AppLog.LogException(ex); }
		}

		#region Channel connections

		/// <summary>Attach to channel and connect any event handlers</summary>
		/// <returns>Connection status</returns>
		private Channels.EConnectionState ChannelsAttach()
		{
			try
			{
				if (mChannels != null)
				{
					Channels.EConnectionState State = mChannels.Connect();
					if (State == Channels.EConnectionState.Connected)  // Open channels
					{
						// Attach event handlers (as required)

						if (mChannels.Application != null)
						{
							mChannels.Application.mEventSubscriptionHeartbeat.Event +=
								new EventHandler<CommunicationsChannel_Application.EventArgsHeartbeat>(EventHandlerHeartbeatApp);
						}


						if (mChannels.Manipulator != null)
						{
							mChannels.Manipulator.mEventSubscriptionHeartbeat.Event +=
								new EventHandler<CommunicationsChannel_Manipulator.EventArgsHeartbeat>(EventHandlerHeartbeatMan);
							mChannels.Manipulator.mEventSubscriptionManipulatorMove.Event +=
								new EventHandler<CommunicationsChannel_Manipulator.EventArgsManipulatorMoveEvent>(EventHandlerManipulatorMoveEvent);
						}

					}
					return State;
				}
			}
			catch (Exception ex) { AppLog.LogException(ex); }
			return Channels.EConnectionState.Error;
		}

		/// <summary>Detach channel and disconnect any event handlers</summary>
		/// <returns>true if OK</returns>
		private bool ChannelsDetach()
		{
			try
			{
				if (mChannels != null)
				{
					// Detach event handlers

					if (mChannels.Application != null)
					{
						mChannels.Application.mEventSubscriptionHeartbeat.Event -=
							new EventHandler<CommunicationsChannel_Application.EventArgsHeartbeat>(EventHandlerHeartbeatApp);
					}


					if (mChannels.Manipulator != null)
					{
						mChannels.Manipulator.mEventSubscriptionHeartbeat.Event -=
							new EventHandler<CommunicationsChannel_Manipulator.EventArgsHeartbeat>(EventHandlerHeartbeatMan);
						mChannels.Manipulator.mEventSubscriptionManipulatorMove.Event -=
							new EventHandler<CommunicationsChannel_Manipulator.EventArgsManipulatorMoveEvent>(EventHandlerManipulatorMoveEvent);
					}

					Thread.Sleep(100); // A breather for events to finish!
					return mChannels.Disconnect(); // Close channels
				}
			}
			catch (Exception ex) { AppLog.LogException(ex); }
			return false;
		}

		#endregion Channel connections

		#region Heartbeat from host

		void EventHandlerHeartbeatApp(object aSender, CommunicationsChannel_Application.EventArgsHeartbeat e)
		{
			try
			{
				if (mChannels == null || mChannels.Application == null)
					return;
				if (this.InvokeRequired)
					this.BeginInvoke((MethodInvoker)delegate { EventHandlerHeartbeatApp(aSender, e); });
				else
				{
					//your code goes here....
				}
			}
			catch (ObjectDisposedException) { } // ignore
			catch (Exception ex) { AppLog.LogException(ex); }
		}

		void EventHandlerHeartbeatMan(object aSender, CommunicationsChannel_Manipulator.EventArgsHeartbeat e)
		{
			try
			{
				if (mChannels == null || mChannels.Manipulator == null)
					return;
				if (this.InvokeRequired)
					this.BeginInvoke((MethodInvoker)delegate { EventHandlerHeartbeatMan(aSender, e); });
				else
				{
					//your code goes here....
				}
			}
			catch (ObjectDisposedException) { } // ignore
			catch (Exception ex) { AppLog.LogException(ex); }
		}

		#endregion Heartbeat from host

		#region STATUS FROM HOST

		#region Manipulator

		void EventHandlerManipulatorMoveEvent(object aSender, CommunicationsChannel_Manipulator.EventArgsManipulatorMoveEvent e)
		{
			try
			{
				if (mChannels == null || mChannels.Manipulator == null)
					return;
				if (this.InvokeRequired)
					this.BeginInvoke((MethodInvoker)delegate { EventHandlerManipulatorMoveEvent(aSender, e); }); // Make it non blocking if called form this UI thread
				else
				{
                    Debug.Print(DateTime.Now.ToString("dd/MM/yyyy H:mm:ss.fff") + " : e.MoveEvent=" + e.MoveEvent.ToString());
					
                    switch (e.MoveEvent)
					{
					case IpcContract.Manipulator.EMoveEvent.HomingStarted:
						// Your code goes here...
						break;
					case IpcContract.Manipulator.EMoveEvent.HomingCompleted:
						// Your code goes here...
						break;
					case IpcContract.Manipulator.EMoveEvent.ManipulatorStartedMoving:
						// Your code goes here...
						break;
					case IpcContract.Manipulator.EMoveEvent.ManipulatorStoppedMoving:
						// Your code goes here...
						break;
					case IpcContract.Manipulator.EMoveEvent.FilamentChangePositionGoStarted:
						// Your code goes here...
						break;
					case IpcContract.Manipulator.EMoveEvent.GoCompleted:
                        // Set mManipulatorGoComplete flag to be true
                        mManipulatorGoComplete = true;
						break;
					case IpcContract.Manipulator.EMoveEvent.GoStarted:
                        // Set mManipulatorGoSignalSent flag to be true 
                        mManipulatorGoSignalSent = true;
						break;
					case IpcContract.Manipulator.EMoveEvent.LoadPositionGoCompleted:
                        // Your code goes here...
						break;
					case IpcContract.Manipulator.EMoveEvent.LoadPositionGoStarted:
						// Your code goes here...
						break;
					case IpcContract.Manipulator.EMoveEvent.Error:
						// Your code goes here...
						break;
					default:
						break;
					}
				}
			}
			catch (Exception ex) { AppLog.LogException(ex); }
		}


		#endregion


        
        #endregion Status from host

        #region User functions

        private void UserForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Attach channels
                mApplicationState = ChannelsAttach();

                if (mApplicationState == Channels.EConnectionState.Connected)
                    Debug.Print(DateTime.Now.ToString("dd/MM/yyyy H:mm:ss.fff") + " : Connected to Inspect-X");
                else
                    Debug.Print(DateTime.Now.ToString("dd/MM/yyyy H:mm:ss.fff") + " : Problem in connecting to Inspect-X");
            }
            catch (Exception ex) { AppLog.LogException(ex); }
        }

        

        private void UserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Detach channels
                ChannelsDetach();

                Debug.Print(DateTime.Now.ToString("dd/MM/yyyy H:mm:ss.fff") + " : Disconnected from Inspect-X");
            }
            catch (Exception ex) { AppLog.LogException(ex); }
        }
        

        private void numericUpDown_RotateAmount_ValueChanged(object sender, EventArgs e)
        {
            mRotateAmount = numericUpDown_RotateAmount.Value;
        }
        
        
        private void btn_Start_Click(object sender, EventArgs e)
        {
            // Initialise a new thread for Manipulator move to run on
            mManipulatorThread = new Thread(ManipulatorMove);
            // Start the thread
            mManipulatorThread.Start();
        }


        #endregion User functions


        #region Manipulator routines

        private void ManipulatorMove()
        {
            // If ApplicationState is not connected then immediately exit the routine
            if (mApplicationState != Channels.EConnectionState.Connected)
                return;

            // For safety, disable the Start button
            this.Invoke((MethodInvoker)delegate 
            { 
                btn_Start.Enabled = false;
                lbl_RotateAmount.Enabled = false;
                numericUpDown_RotateAmount.Enabled = false;
            });

            // Variable for current manipulator position
            float aManipulatorCurrentPosition;

            // Variable for demanded manipulator position
            float aManipulatorDemandedPosition;

            // Set Movement flags to be false
            mManipulatorGoSignalSent = false;
            mManipulatorGoComplete = false;

            // Look up current position
            aManipulatorCurrentPosition = mChannels.Manipulator.Axis.Position(mRotateAxis);

            // Calculate new demanded position
            aManipulatorDemandedPosition = aManipulatorCurrentPosition + (float)mRotateAmount;

            // Set target position of rotate axis to be demanded position
            mChannels.Manipulator.Axis.Target(mRotateAxis, aManipulatorDemandedPosition);

            // Tell manipulator to move
            mChannels.Manipulator.Axis.Go(mRotateAxis);

            // while both movement flags are still not positive, then wait
            while (!mManipulatorGoSignalSent || !mManipulatorGoComplete)
                Thread.Sleep(10);

            // Re-enable the Start button
            this.Invoke((MethodInvoker)delegate
            {
                btn_Start.Enabled = true;
                lbl_RotateAmount.Enabled = true;
                numericUpDown_RotateAmount.Enabled = true;
            });
        }

        #endregion Manipulator routines




    }
}
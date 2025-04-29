using Gtk;
using NBitcoin;
using System;

namespace GhostBTC.UI
{
    public class GtkUI
    {
        private readonly Wallet _wallet;
        private Window _mainWindow;
        private Label _balanceLabel;
        
        public GtkUI(Wallet wallet)
        {
            _wallet = wallet;
            Application.Init();
            _mainWindow = new Window("GhostBTC Wallet");
            _mainWindow.SetDefaultSize(400, 300);
            _mainWindow.DeleteEvent += OnWindowDelete;
            
            InitializeUI();
        }

        private void InitializeUI()
        {
            var vbox = new VBox(false, 5);
            
            // Balance display
            _balanceLabel = new Label("Balance: Loading...");
            vbox.PackStart(_balanceLabel, false, false, 0);
            
            // New Address button
            var newAddressBtn = new Button("Generate New Address");
            newAddressBtn.Clicked += OnNewAddressClicked;
            vbox.PackStart(newAddressBtn, false, false, 0);
            
            // Send Bitcoin button
            var sendBtn = new Button("Send Bitcoin");
            sendBtn.Clicked += OnSendClicked;
            vbox.PackStart(sendBtn, false, false, 0);
            
            _mainWindow.Add(vbox);
        }

        private void OnNewAddressClicked(object sender, EventArgs e)
        {
            var address = _wallet.GetNewAddress();
            var dialog = new MessageDialog(
                _mainWindow,
                DialogFlags.Modal,
                MessageType.Info,
                ButtonsType.Ok,
                $"New Address: {address}");
            dialog.Run();
            dialog.Destroy();
        }

        private void OnSendClicked(object sender, EventArgs e)
        {
            // Implement send dialog
            var dialog = new MessageDialog(
                _mainWindow,
                DialogFlags.Modal,
                MessageType.Info,
                ButtonsType.Ok,
                "Send functionality would go here");
            dialog.Run();
            dialog.Destroy();
        }

        private void OnWindowDelete(object sender, DeleteEventArgs args)
        {
            Application.Quit();
            args.RetVal = true;
        }

        public void Run()
        {
            _mainWindow.ShowAll();
            Application.Run();
        }
    }
}
using System;
using System.Collections;
using ArduinoBluetoothAPI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BluetoothManager : MonoBehaviour
{
    public DrawManager DrawManager;
    public GCodeManager GCodeManager;

    public Button PlusXButton;
    public Button PlusYButton;
    public Button PlusZButton;
    public Button MinusXButton;
    public Button MinusYButton;
    public Button MinusZButton;

    public TMP_InputField StepInputField;
    public TMP_InputField SpeedInputField;

    public Button Set0Button;
    public Button Reset0Button;

    public Button ConnectButton;
    public TMP_Text ConnectButtonText;
    public Button DrawButton;
    public Button ClearButton;

    private BluetoothHelper bluetoothHelper;

    private void Start()
    {
        ConnectButton.onClick.AddListener(Connect);
        ClearButton.onClick.AddListener(Clear);
        DrawButton.onClick.AddListener(Draw);

        Set0Button.onClick.AddListener(Set0);
        Reset0Button.onClick.AddListener(Reset0);

        PlusXButton.onClick.AddListener(PlusX);
        PlusYButton.onClick.AddListener(PlusY);
        PlusZButton.onClick.AddListener(PlusZ);

        MinusXButton.onClick.AddListener(MinusX);
        MinusYButton.onClick.AddListener(MinusY);
        MinusZButton.onClick.AddListener(MinusZ);

        SpeedInputField.onEndEdit.AddListener(OnEditSpeed);

        try
        {
            bluetoothHelper = BluetoothHelper.GetInstance("HC-06");
            bluetoothHelper.setTerminatorBasedStream("\n");
            bluetoothHelper.OnConnected += OnBluetoothConnected;

            bluetoothHelper.OnDataReceived += helper =>
            {
            };
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    private void Set0()
    {
        if (bluetoothHelper != null && bluetoothHelper.isConnected())
        {
            bluetoothHelper.SendData("G21 G90 G0 Z5");
            bluetoothHelper.SendData("G90 G28 X0 Y0");
            bluetoothHelper.SendData("G90 G28 Z0");
        }
    }

    private void Reset0()
    {
        if (bluetoothHelper != null && bluetoothHelper.isConnected())
        {
            bluetoothHelper.SendData("G92 X0 Y0 Z0");
        }
    }

    private void OnEditSpeed(string speed)
    {
        if (bluetoothHelper != null && bluetoothHelper.isConnected())
        {
            var gCode = $"$4={speed}";
            bluetoothHelper.SendData(gCode);
        }
    }

    private void PlusX()
    {
        if (bluetoothHelper != null && bluetoothHelper.isConnected())
        {
            var step = StepInputField.text;
            var gCode = $"G21 G91 G0 X{step}";
            bluetoothHelper.SendData(gCode);
        }
    }

    private void PlusY()
    {
        if (bluetoothHelper != null && bluetoothHelper.isConnected())
        {
            var step = StepInputField.text;
            var gCode = $"G21 G91 G0 Y{step}";
            bluetoothHelper.SendData(gCode);
        }
    }

    private void PlusZ()
    {
        if (bluetoothHelper != null && bluetoothHelper.isConnected())
        {
            var step = StepInputField.text;
            var gCode = $"G21 G91 G0 Z{step}";
            bluetoothHelper.SendData(gCode);
        }
    }

    private void MinusX()
    {
        if (bluetoothHelper != null && bluetoothHelper.isConnected())
        {
            var step = StepInputField.text;
            var gCode = $"G21 G91 G0 X-{step}";
            bluetoothHelper.SendData(gCode);
        }
    }

    private void MinusY()
    {
        if (bluetoothHelper != null && bluetoothHelper.isConnected())
        {
            var step = StepInputField.text;
            var gCode = $"G21 G91 G0 Y-{step}";
            bluetoothHelper.SendData(gCode);
        }
    }

    private void MinusZ()
    {
        if (bluetoothHelper != null && bluetoothHelper.isConnected())
        {
            var step = StepInputField.text;
            var gCode = $"G21 G91 G0 Z-{step}";
            bluetoothHelper.SendData(gCode);
        }
    }

    private void Draw()
    {
        if (bluetoothHelper != null && bluetoothHelper.isConnected())
            StartCoroutine(SendGCode());
    }

    private IEnumerator SendGCode()
    {
        var gCode = GCodeManager.GenerateGCode();
        foreach (var code in gCode)
        {
            bluetoothHelper.SendData(code);
            yield return new WaitForSeconds(1f);
        }
    }

    private void Clear()
    {
        DrawManager.Clear();
    }

    private void OnBluetoothConnected(BluetoothHelper helper)
    {
        try
        {
            helper.StartListening();
            ConnectButtonText.text = "Desconectar";
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void OnDestroy()
    {
        bluetoothHelper?.Disconnect();
    }

    private void Connect()
    {
        if (bluetoothHelper == null)
            return;

        if (!bluetoothHelper.isConnected() && bluetoothHelper.isDevicePaired())
            bluetoothHelper.Connect();
        else if (bluetoothHelper.isConnected())
        {
            ConnectButtonText.text = "Conectar";
            bluetoothHelper.Disconnect();
        }
    }
}
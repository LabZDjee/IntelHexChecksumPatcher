using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using IntelHexUtilityNS;

namespace IntelHexChecksumPatcher
{
  public class MainForm : Form
  {
    private GroupBox checksumOpGroup;
    private RadioButton rButtonCheckAdd;
    private RadioButton rButtonCheckUpdate;
    private Button buttonFiledialog;
    private TextBox filenameTextBox;
    private Label resultLabel;
    private TextBox resultTextBox;

    public MainForm()
    {
      SuspendLayout();
      ClientSize = new System.Drawing.Size(550, 375);
      MaximizeBox = false;
      Text = "Intel Hex Checksum Patcher";
      FormBorderStyle = FormBorderStyle.FixedDialog;
      checksumOpGroup = new System.Windows.Forms.GroupBox();
      rButtonCheckAdd = new System.Windows.Forms.RadioButton();
      rButtonCheckUpdate = new System.Windows.Forms.RadioButton();
      checksumOpGroup.Controls.Add(this.rButtonCheckAdd);
      checksumOpGroup.Controls.Add(this.rButtonCheckUpdate);
      checksumOpGroup.Location = new System.Drawing.Point(190, 1);
      checksumOpGroup.Size = new System.Drawing.Size(200, 36);
      checksumOpGroup.Text = "  Checksum  ";
      rButtonCheckAdd.Location = new System.Drawing.Point(10, 16);
      rButtonCheckAdd.Size = new System.Drawing.Size(85, 17);
      rButtonCheckAdd.Text = "Append";
      rButtonCheckAdd.Checked = false;
      rButtonCheckUpdate.Location = new System.Drawing.Point(110, 16);
      rButtonCheckUpdate.Size = new System.Drawing.Size(85, 17);
      rButtonCheckUpdate.Text = "Update";
      rButtonCheckUpdate.Checked = true;
      Controls.Add(checksumOpGroup);
      buttonFiledialog = new Button();
      buttonFiledialog.Location = new Point(30, 35);
      buttonFiledialog.Text = "Intel Hex";
      buttonFiledialog.Click += new EventHandler(OnFileDialogClick);
      Controls.Add(buttonFiledialog);
      filenameTextBox = new TextBox();
      filenameTextBox.ReadOnly = true;
      filenameTextBox.Multiline = false;
      filenameTextBox.WordWrap = false;
      filenameTextBox.TextAlign = HorizontalAlignment.Right;
      filenameTextBox.Location = new Point(110, 39);
      filenameTextBox.Size = new Size(420, 20);
      Controls.Add(filenameTextBox);
      resultLabel = new Label();
      resultLabel.Location = new Point(30, 60);
      resultLabel.Size = new Size(120, 17);
      resultLabel.Text = "Result:";
      Controls.Add(resultLabel);
      resultTextBox = new TextBox();
      resultTextBox.ReadOnly = true;
      resultTextBox.Multiline = true;
      resultTextBox.Location = new Point(30, 78);
      resultTextBox.Size = new Size(500, 280);
      resultTextBox.ScrollBars = ScrollBars.Both;
      resultTextBox.WordWrap = false;
      Controls.Add(resultTextBox);
      ResumeLayout(false);
    }

    [STAThread]
    public static void Main(string[] args)
    {
      Application.Run(new MainForm());
    }

    void OnFileDialogClick(object sender, System.EventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "Intel Hex (*.hex)|*.hex|All Files (*.*)|*.*";
      openFileDialog.Multiselect = false;
      openFileDialog.RestoreDirectory = false;
      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;

      bool bAddChecksum = rButtonCheckAdd.Checked;
      string filepath = openFileDialog.FileName.Trim();
      if (filepath != string.Empty) {
        filenameTextBox.Text = filepath;
        filenameTextBox.SelectionStart = filenameTextBox.Text.Length - 1;
        string [] contents = File.ReadAllLines(filepath);
        resultTextBox.Text = "";
        int lineNumber = 1;
        foreach (string line in contents) {
          string resultLine = string.Empty;
          try {
            byte [] lineAsByteArray = IntelHexUtility.TurnHexLineIntoByteArray(line, bAddChecksum);
            if(!bAddChecksum) {
              if (lineAsByteArray.Length<1)
                throw new Exception("cannot update an empty IntelHex string");
              lineAsByteArray[lineAsByteArray.Length-1]=0;
            }
            byte checksum = IntelHexUtility.CalculateChecksum(lineAsByteArray);
            lineAsByteArray[lineAsByteArray.Length - 1] = checksum;
            resultLine = IntelHexUtility.BuildIntelHexFromByteArray(lineAsByteArray);

          } catch (Exception error) {
            resultTextBox.Text = string.Format("ERROR at line {1} in HexFile: {0}", error.Message, lineNumber);
            break;
          }
          resultTextBox.Text += resultLine + "\r\n";
          lineNumber++;
        }
      }
      openFileDialog.Dispose();
    }
  }

}

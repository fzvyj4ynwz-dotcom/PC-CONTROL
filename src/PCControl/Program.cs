using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace PCControl;
public class Program {
[StructLayout(LayoutKind.Sequential)] struct Memory { public uint Length, Load; public ulong Total, Available, PageTotal, PageAvailable, VirtualTotal, VirtualAvailable, Extended; }
[DllImport("kernel32.dll")] static extern bool GlobalMemoryStatusEx(ref Memory m);
[DllImport("kernel32.dll")] static extern bool GetSystemTimes(out long idle,out long kernel,out long user);
[STAThread] public static void Main() {
var app=new Application(); var panel=new StackPanel {Margin=new Thickness(32)};
var window=new Window {Title="PC Control Center",Width=960,Height=680,Background=new SolidColorBrush(Color.FromRgb(18,20,28)),Content=new ScrollViewer{Content=panel,VerticalScrollBarVisibility=ScrollBarVisibility.Auto}};
TextBlock Label(string value,double size){var t=new TextBlock{Text=value,FontSize=size,Foreground=Brushes.White,Margin=new Thickness(0,8,0,8)};panel.Children.Add(t);return t;}
Label("PC Control Center",36); Label("PANORAMICA SISTEMA",15);
var cpu=Label("CPU —",28);var ram=Label("RAM —",28);var disk=Label("Disco —",24);var status=Label("",16);
long pi=0,pt=0;
var timer=new DispatcherTimer{Interval=TimeSpan.FromSeconds(2)};
timer.Tick+=(s,e)=>{try{if(GetSystemTimes(out long i,out long k,out long u)){long t=k+u;if(pt>0&&t>pt)cpu.Text=$"CPU  {Math.Clamp(100.0*(1-(double)(i-pi)/(t-pt)),0,100):F0}%";pi=i;pt=t;}var m=new Memory{Length=(uint)Marshal.SizeOf<Memory>()};if(GlobalMemoryStatusEx(ref m))ram.Text=$"RAM  {m.Load}%";var d=new System.IO.DriveInfo(System.IO.Path.GetPathRoot(Environment.SystemDirectory)!);disk.Text=$"Disco  {d.AvailableFreeSpace/1073741824.0:F1} GB liberi";}catch(Exception ex){status.Text=ex.Message;}};
foreach(var item in new[]{("Modalità gioco","ms-settings:gaming-gamemode"),("App in avvio","ms-settings:startupapps"),("Archiviazione","ms-settings:storagesense")}){var b=new Button{Content=item.Item1,Padding=new Thickness(12),Margin=new Thickness(0,5,0,5)};b.Click+=(s,e)=>{try{Process.Start(new ProcessStartInfo(item.Item2){UseShellExecute=true});}catch(Exception ex){status.Text=ex.Message;}};panel.Children.Add(b);}
var diagnosticButton=new Button{Content="Diagnostica avanzata",Padding=new Thickness(12)};panel.Children.Add(diagnosticButton);
var report=Label("",14);
diagnosticButton.Click+=async(s,e)=>{diagnosticButton.IsEnabled=false;try{report.Text=await System.Threading.Tasks.Task.Run(Diagnostics.Snapshot);}catch(Exception ex){report.Text=ex.Message;}finally{diagnosticButton.IsEnabled=true;}};
Label("Versione di sviluppo — nessuna ottimizzazione automatica attiva",14);timer.Start();window.Closed+=(s,e)=>timer.Stop();app.Run(window);
}}

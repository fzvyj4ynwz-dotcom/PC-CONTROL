using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
namespace PCControl;
public static class Diagnostics {
 public static string Snapshot() {
 var b=new StringBuilder();
 b.AppendLine($"Windows: {Environment.OSVersion.Version}");
 b.AppendLine($"Processori logici: {Environment.ProcessorCount}");
 b.AppendLine($"Architettura: {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}");
 b.AppendLine("\nProcessi per memoria:");
 var rows=new List<(string Name,long Bytes)>();
 foreach(var p in Process.GetProcesses()) { using(p) {try{rows.Add((p.ProcessName,p.WorkingSet64));}catch{}} }
 foreach(var r in rows.OrderByDescending(x=>x.Bytes).Take(12)) b.AppendLine($"{r.Name}: {r.Bytes/1048576.0:F0} MB");
 b.AppendLine("\nSchede di rete:");
 foreach(var n in NetworkInterface.GetAllNetworkInterfaces()) {try{b.AppendLine($"{n.Name}: {n.OperationalStatus}, {n.Speed/1000000.0:F0} Mbps (velocità collegamento)");}catch{}}
 return b.ToString();
 }
}

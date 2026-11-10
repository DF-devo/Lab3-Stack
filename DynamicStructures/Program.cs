using System.Diagnostics;
using System.Globalization;

namespace DynamicStructures;

sealed class Node<T>(T value, Node<T>? next = null) { public T Value = value; public Node<T>? Next = next; }
sealed class LinkedList<T> : IEnumerable<T>
{
    private Node<T>? head, tail; public int Count { get; private set; } public bool IsEmpty => head is null;
    public void AddFirst(T x) { head = new Node<T>(x, head); if (tail is null) tail = head; Count++; }
    public void AddLast(T x) { var n=new Node<T>(x); if(tail is null) head=tail=n; else {tail.Next=n;tail=n;} Count++; }
    public bool RemoveFirst(out T value) { if(head is null){value=default!;return false;} value=head.Value;head=head.Next;if(head is null)tail=null;Count--;return true; }
    public bool TryFirst(out T value) { if(head is null){value=default!;return false;} value=head.Value;return true; }
    public void Reverse(){Node<T>? prev=null,cur=head;tail=head;while(cur is not null){var n=cur.Next;cur.Next=prev;prev=cur;cur=n;}head=prev;}
    public void MoveLastToFront(){if(head is null||head==tail)return;var p=head;while(p.Next!=tail)p=p.Next;tail!.Next=head;head=tail;tail=p;tail.Next=null;}
    public void MoveFirstToEnd(){if(head is null||head==tail)return;var n=head;head=head.Next;n.Next=null;tail!.Next=n;tail=n;}
    public int DistinctCount()=>this.Distinct().Count();
    public void RemoveDuplicates(){var seen=new HashSet<T>();Node<T>? prev=null,cur=head;while(cur is not null){if(!seen.Add(cur.Value)){if(prev is null)head=cur.Next;else prev.Next=cur.Next;if(cur==tail)tail=prev;Count--;}else prev=cur;cur=cur.Next;}}
    public void InsertSorted(T x,IComparer<T>? cmp=null){cmp??=Comparer<T>.Default;if(head is null||cmp.Compare(x,head.Value)<=0){AddFirst(x);return;}var p=head;while(p.Next is not null&&cmp.Compare(p.Next.Value,x)<=0)p=p.Next;p.Next=new Node<T>(x,p.Next);if(p.Next.Next is null)tail=p.Next;Count++;}
    public void RemoveAll(T x){var a=this.Where(v=>!EqualityComparer<T>.Default.Equals(v,x)).ToArray();Clear();foreach(var v in a)AddLast(v);}
    public void Clear(){head=tail=null;Count=0;}
    public void Append(LinkedList<T> other){foreach(var x in other)AddLast(x);}
    public LinkedList<T> SplitAfterFirst(T x){var b=new LinkedList<T>();var p=head;while(p is not null&&!EqualityComparer<T>.Default.Equals(p.Value,x))p=p.Next;if(p is null)return b;var rest=p.Next;p.Next=null;tail=p;b.head=rest;b.tail=rest;if(rest is not null){b.Count=1;while(b.tail!.Next is not null){b.tail=b.tail.Next;b.Count++;}}Count-=b.Count;return b;}
    public void Duplicate(){var a=this.ToArray();foreach(var x in a)AddLast(x);}
    public override string ToString()=>string.Join(" ",this);
    public IEnumerator<T> GetEnumerator(){for(var p=head;p is not null;p=p.Next)yield return p.Value;} System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()=>GetEnumerator();
}
sealed class LinkedStack<T>{private readonly LinkedList<T> l=new();public bool IsEmpty=>l.IsEmpty;public void Push(T x)=>l.AddFirst(x);public bool Pop(out T x)=>l.RemoveFirst(out x);public bool Top(out T x)=>l.TryFirst(out x);public IEnumerable<T> Items=>l;public override string ToString()=>l.ToString();}
sealed class LinkedQueue<T>{private readonly LinkedList<T> l=new();public bool IsEmpty=>l.IsEmpty;public void Enqueue(T x)=>l.AddLast(x);public bool Dequeue(out T x)=>l.RemoveFirst(out x);public bool Front(out T x)=>l.TryFirst(out x);public IEnumerable<T> Items=>l;public override string ToString()=>l.ToString();}
static class Program
{
 static void Main(){ApplicationConfiguration.Initialize();Application.Run(new MainForm());}
 static string[] ReadOps()=>File.ReadAllText("input.txt").Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries);
 static void StackFile(){var s=new LinkedStack<string>();var sw=Stopwatch.StartNew();foreach(var t in ReadOps()){var a=t.Split(',',2);switch(a[0]){case "1":if(a.Length==2)s.Push(a[1]);else Console.WriteLine("Для Push нужен формат 1,значение");break;case "2":Console.WriteLine(s.Pop(out var v)?$"Pop: {v}":"Стек пуст");break;case "3":Console.WriteLine(s.Top(out var q)?$"Top: {q}":"Стек пуст");break;case "4":Console.WriteLine(s.IsEmpty);break;case "5":Console.WriteLine(s);break;default:Console.WriteLine($"Неизвестная команда {t}");break;}}sw.Stop();Console.WriteLine($"Время чтения и выполнения: {sw.Elapsed.TotalMilliseconds:F3} мс");}
 static void QueueFile(){var a=new LinkedQueue<string>();var b=new Queue<string>();var ops=ReadOps();var sw=Stopwatch.StartNew();foreach(var t in ops){var z=t.Split(',',2);switch(z[0]){case "1":if(z.Length==2){a.Enqueue(z[1]);b.Enqueue(z[1]);}break;case "2":a.Dequeue(out _);if(b.Count>0)b.Dequeue();break;case "3":Console.WriteLine(a.Front(out var x)?$"Front: {x}":"Очередь пуста");break;case "4":Console.WriteLine(a.IsEmpty);break;case "5":Console.WriteLine(a);break;}}sw.Stop();Console.WriteLine($"Связный список: {sw.Elapsed.TotalMilliseconds:F3} мс");sw.Restart();foreach(var t in ops){var z=t.Split(',',2);switch(z[0]){case "1":if(z.Length==2)b.Enqueue(z[1]);break;case "2":if(b.Count>0)b.Dequeue();break;case "3":Console.WriteLine(b.Count>0?$"Front: {b.Peek()}":"Очередь пуста");break;case "4":Console.WriteLine(b.Count==0);break;case "5":Console.WriteLine(string.Join(" ",b));break;}}sw.Stop();Console.WriteLine($"Queue<T>: {sw.Elapsed.TotalMilliseconds:F3} мс");}
 static void RunPostfixFile(){Console.WriteLine(Evaluate(File.ReadAllText("postfix.txt")));Console.WriteLine("Сложность вычисления: O(n) времени и O(n) памяти.");}
 public static double Evaluate(string text){var s=new Stack<double>();foreach(var t in text.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries)){if(double.TryParse(t,NumberStyles.Float,CultureInfo.InvariantCulture,out var n)){s.Push(n);continue;}double Pop()=>s.Count>0?s.Pop():throw new FormatException("Недостаточно операндов");switch(t.ToLowerInvariant()){case "+":{var b=Pop();s.Push(Pop()+b);break;}case "-":{var b=Pop();s.Push(Pop()-b);break;}case "*":{var b=Pop();s.Push(Pop()*b);break;}case "/":case ":":{var b=Pop();s.Push(Pop()/b);break;}case "^":{var b=Pop();s.Push(Math.Pow(Pop(),b));break;}case "ln":s.Push(Math.Log(Pop()));break;case "cos":s.Push(Math.Cos(Pop()));break;case "sin":s.Push(Math.Sin(Pop()));break;case "sqrt":s.Push(Math.Sqrt(Pop()));break;default:throw new FormatException($"Неизвестный токен {t}");}}if(s.Count!=1)throw new FormatException("Некорректное postfix-выражение");return s.Pop();}
 static IEnumerable<string> Tokens(string s){var m=System.Text.RegularExpressions.Regex.Matches(s,@"\d+(?:\.\d+)?|[A-Za-z]+|[()+*/^:\-]");foreach(System.Text.RegularExpressions.Match x in m)yield return x.Value;}
 static int Prec(string x)=>x switch{"+" or "-"=>1,"*" or "/" or ":"=>2,"^"=>3,_=>0};
 public static string ToPostfix(string expr){var output=new List<string>();var ops=new Stack<string>();foreach(var t in Tokens(expr)){if(double.TryParse(t,NumberStyles.Float,CultureInfo.InvariantCulture,out _))output.Add(t);else if(t is "sin" or "cos" or "ln" or "sqrt")ops.Push(t);else if(t=="(")ops.Push(t);else if(t==")"){while(ops.Count>0&&ops.Peek()!="(")output.Add(ops.Pop());if(ops.Count==0)throw new FormatException("Несогласованные скобки");ops.Pop();if(ops.Count>0&&ops.Peek() is "sin" or "cos" or "ln" or "sqrt")output.Add(ops.Pop());}else{while(ops.Count>0&&ops.Peek()!="("&&(Prec(ops.Peek())>Prec(t)||(Prec(ops.Peek())==Prec(t)&&t!="^")))output.Add(ops.Pop());ops.Push(t);}}while(ops.Count>0){var t=ops.Pop();if(t=="(")throw new FormatException("Несогласованные скобки");output.Add(t);}return string.Join(" ",output);}
 static void ConvertInteractive(){Console.Write("Формула: ");var p=ToPostfix(Console.ReadLine()??"");Console.WriteLine($"Postfix: {p}\nРезультат: {Evaluate(p)}");}
 static void ListDemo(){var l=new LinkedList<int>();Console.WriteLine("Введите целые числа через пробел:");foreach(var t in (Console.ReadLine()??"").Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries))if(int.TryParse(t,out var n))l.AddLast(n);Console.WriteLine($"Список: {l}\nРазличных: {l.DistinctCount()}");l.RemoveDuplicates();Console.WriteLine($"После удаления повторов: {l}");l.Reverse();Console.WriteLine($"После разворота: {l}");}
}





using System.Diagnostics;
using DynamicStructuresDemo.Structures;

namespace DynamicStructuresDemo;

/// <summary>
/// Main window. The UI is built in code (no designer file) to keep the project easy to read.
/// Tabs: doubly linked list, dynamic stack, dynamic queue and a static vs dynamic comparison.
/// </summary>
public sealed class MainForm : Form
{
    // Memory model used for the estimates (64-bit runtime, T = int).
    // Singly linked node: 8 (object header) + 8 (method table) + 8 (int padded) + 8 (Next) = 32 bytes.
    private const int DataBytes = sizeof(int);
    private const int NodeBytes = 32;

    private readonly DoublyLinkedList<int> _list = new();
    private readonly DynamicStack<int> _stack = new();
    private readonly DynamicQueue<int> _queue = new();

    private TabUi _listUi = null!;
    private TabUi _stackUi = null!;
    private TabUi _queueUi = null!;

    private NumericUpDown _elementsBox = null!;
    private NumericUpDown _capacityBox = null!;
    private ComboBox _kindBox = null!;
    private ListView _resultsView = null!;
    private TextBox _summaryBox = null!;

    public MainForm()
    {
        Text = "Tarea 3.2 - Estructuras dinámicas: Listas, Pilas y Colas";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(940, 640);
        MinimumSize = new Size(760, 520);

        var tabs = new TabControl { Dock = DockStyle.Fill };

        _listUi = new TabUi("Lista doblemente enlazada", showPosition: true, new[]
        {
            new Command("Agregar al inicio", () => Run(_listUi, RefreshList, () =>
            {
                int v = ReadInt(_listUi.Input, "Valor");
                _list.AddFirst(v);
                _listUi.Log($"Agregado al inicio: {v}");
            })),
            new Command("Agregar al final", () => Run(_listUi, RefreshList, () =>
            {
                int v = ReadInt(_listUi.Input, "Valor");
                _list.AddLast(v);
                _listUi.Log($"Agregado al final: {v}");
            })),
            new Command("Insertar en índice", () => Run(_listUi, RefreshList, () =>
            {
                int v = ReadInt(_listUi.Input, "Valor");
                int i = ReadInt(_listUi.Position, "Índice");
                _list.InsertAt(i, v);
                _listUi.Log($"Insertado {v} en el índice {i}");
            })),
            new Command("Eliminar primero", () => Run(_listUi, RefreshList, () =>
                _listUi.Log($"Primero eliminado: {_list.RemoveFirst()}"))),
            new Command("Eliminar último", () => Run(_listUi, RefreshList, () =>
                _listUi.Log($"Último eliminado: {_list.RemoveLast()}"))),
            new Command("Eliminar en índice", () => Run(_listUi, RefreshList, () =>
            {
                int i = ReadInt(_listUi.Position, "Índice");
                _listUi.Log($"Eliminado en el índice {i}: {_list.RemoveAt(i)}");
            })),
            new Command("Eliminar valor", () => Run(_listUi, RefreshList, () =>
            {
                int v = ReadInt(_listUi.Input, "Valor");
                _listUi.Log(_list.Remove(v) ? $"Valor {v} eliminado" : $"Valor {v} no encontrado");
            })),
            new Command("Buscar", () => Run(_listUi, RefreshList, () =>
            {
                int v = ReadInt(_listUi.Input, "Valor");
                int idx = _list.IndexOf(v);
                _listUi.Log(idx >= 0 ? $"Valor {v} encontrado en el índice {idx}" : $"Valor {v} no encontrado");
            })),
            new Command("Invertir", () => Run(_listUi, RefreshList, () =>
            {
                _list.Reverse();
                _listUi.Log("Lista invertida (punteros intercambiados en el lugar)");
            })),
            new Command("Limpiar", () => Run(_listUi, RefreshList, () =>
            {
                _list.Clear();
                _listUi.Log("Lista vaciada");
            })),
        });

        _stackUi = new TabUi("Pila dinámica (LIFO)", showPosition: false, new[]
        {
            new Command("Apilar (Push)", () => Run(_stackUi, RefreshStack, () =>
            {
                int v = ReadInt(_stackUi.Input, "Valor");
                _stack.Push(v);
                _stackUi.Log($"Apilado: {v}");
            })),
            new Command("Desapilar (Pop)", () => Run(_stackUi, RefreshStack, () =>
                _stackUi.Log($"Desapilado: {_stack.Pop()}"))),
            new Command("Ver cima", () => Run(_stackUi, RefreshStack, () =>
                _stackUi.Log($"Cima: {_stack.Peek()}"))),
            new Command("Limpiar", () => Run(_stackUi, RefreshStack, () =>
            {
                _stack.Clear();
                _stackUi.Log("Pila vaciada");
            })),
        });

        _queueUi = new TabUi("Cola dinámica (FIFO)", showPosition: false, new[]
        {
            new Command("Encolar", () => Run(_queueUi, RefreshQueue, () =>
            {
                int v = ReadInt(_queueUi.Input, "Valor");
                _queue.Enqueue(v);
                _queueUi.Log($"Encolado: {v}");
            })),
            new Command("Desencolar", () => Run(_queueUi, RefreshQueue, () =>
                _queueUi.Log($"Desencolado: {_queue.Dequeue()}"))),
            new Command("Ver frente", () => Run(_queueUi, RefreshQueue, () =>
                _queueUi.Log($"Frente: {_queue.Peek()}"))),
            new Command("Limpiar", () => Run(_queueUi, RefreshQueue, () =>
            {
                _queue.Clear();
                _queueUi.Log("Cola vaciada");
            })),
        });

        tabs.TabPages.Add(_listUi.Page);
        tabs.TabPages.Add(_stackUi.Page);
        tabs.TabPages.Add(_queueUi.Page);
        tabs.TabPages.Add(BuildComparisonTab());
        Controls.Add(tabs);

        RefreshList();
        RefreshStack();
        RefreshQueue();
    }

    // ------------------------------------------------------------------
    // Display refresh
    // ------------------------------------------------------------------

    private void RefreshList()
    {
        _listUi.Display.Text = _list.IsEmpty
            ? "(lista vacía)   cabeza = null, cola = null"
            : "cabeza -> " + string.Join(" <-> ", _list.Select(v => $"[{v}]")) + " <- cola"
              + Environment.NewLine + $"Cantidad = {_list.Count}";
    }

    private void RefreshStack()
    {
        _stackUi.Display.Text = _stack.IsEmpty
            ? "(pila vacía)   cima = null"
            : "CIMA -> " + string.Join(" ", _stack.Select(v => $"[{v}]")) + " <- FONDO"
              + Environment.NewLine + $"Cantidad = {_stack.Count}";
    }

    private void RefreshQueue()
    {
        _queueUi.Display.Text = _queue.IsEmpty
            ? "(cola vacía)   frente = null, final = null"
            : "FRENTE -> " + string.Join(" ", _queue.Select(v => $"[{v}]")) + " <- FINAL"
              + Environment.NewLine + $"Cantidad = {_queue.Count}";
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private static void Run(TabUi ui, Action refresh, Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex) when (ex is InvalidOperationException
                                      or ArgumentOutOfRangeException
                                      or FormatException)
        {
            ui.Log("ERROR: " + ex.Message);
        }

        refresh();
        ui.Input.SelectAll();
        ui.Input.Focus();
    }

    private static int ReadInt(TextBox box, string name)
    {
        if (!int.TryParse(box.Text.Trim(), out int value))
            throw new FormatException($"'{name}' debe ser un número entero válido.");

        return value;
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes >= 1024 * 1024)
            return $"{bytes / (1024.0 * 1024.0):F2} MB ({bytes:N0} B)";
        if (bytes >= 1024)
            return $"{bytes / 1024.0:F2} KB ({bytes:N0} B)";
        return $"{bytes:N0} B";
    }

    // ------------------------------------------------------------------
    // Static vs dynamic comparison
    // ------------------------------------------------------------------

    private TabPage BuildComparisonTab()
    {
        var page = new TabPage("Estático vs Dinámico") { Padding = new Padding(8) };

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));

        var bar = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };

        bar.Controls.Add(new Label { Text = "Estructura:", AutoSize = true, Margin = new Padding(3, 8, 3, 0) });
        _kindBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 90 };
        _kindBox.Items.AddRange(new object[] { "Pila", "Cola" });
        _kindBox.SelectedIndex = 0;
        bar.Controls.Add(_kindBox);

        bar.Controls.Add(new Label { Text = "Elementos a guardar:", AutoSize = true, Margin = new Padding(12, 8, 3, 0) });
        _elementsBox = new NumericUpDown { Minimum = 1, Maximum = 5_000_000, Value = 1_000, Width = 90 };
        bar.Controls.Add(_elementsBox);

        bar.Controls.Add(new Label { Text = "Capacidad estática:", AutoSize = true, Margin = new Padding(12, 8, 3, 0) });
        _capacityBox = new NumericUpDown { Minimum = 1, Maximum = 5_000_000, Value = 5_000, Width = 90 };
        bar.Controls.Add(_capacityBox);

        var runButton = new Button { Text = "Ejecutar comparación", AutoSize = true, Margin = new Padding(12, 3, 3, 3) };
        runButton.Click += (_, _) => RunComparison();
        bar.Controls.Add(runButton);

        _resultsView = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true
        };
        _resultsView.Columns.Add("Métrica", 280);
        _resultsView.Columns.Add("Estático (arreglo)", 270);
        _resultsView.Columns.Add("Dinámico (nodos)", 270);

        _summaryBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Text = "Elija el número de elementos y la capacidad fija de la estructura estática, y presione \"Ejecutar comparación\"."
        };

        layout.Controls.Add(bar, 0, 0);
        layout.Controls.Add(_resultsView, 0, 1);
        layout.Controls.Add(_summaryBox, 0, 2);
        page.Controls.Add(layout);
        return page;
    }

    private void RunComparison()
    {
        int elements = (int)_elementsBox.Value;
        int capacity = (int)_capacityBox.Value;
        bool isStack = _kindBox.SelectedIndex == 0;

        Cursor = Cursors.WaitCursor;
        try
        {
            // Warm-up so that JIT compilation does not distort the first measurement.
            Measure(() => isStack ? new StaticStack<int>(16) : new StaticQueue<int>(16), 16);
            Measure(() => isStack ? new DynamicStack<int>() : new DynamicQueue<int>(), 16);

            BenchmarkResult st = Measure(
                () => isStack ? new StaticStack<int>(capacity) : new StaticQueue<int>(capacity), elements);
            BenchmarkResult dy = Measure(
                () => isStack ? new DynamicStack<int>() : new DynamicQueue<int>(), elements);

            // Estimates based on the memory model (see the constants at the top of the class).
            long staticReserved = (long)capacity * DataBytes;
            long staticUseful = (long)st.Stored * DataBytes;
            long staticWasted = staticReserved - staticUseful;

            long dynamicUseful = (long)dy.Stored * DataBytes;
            long dynamicTotal = (long)dy.Stored * NodeBytes;
            long dynamicOverhead = dynamicTotal - dynamicUseful;

            double staticEfficiency = staticReserved == 0 ? 0 : 100.0 * staticUseful / staticReserved;
            double dynamicEfficiency = dynamicTotal == 0 ? 0 : 100.0 * dynamicUseful / dynamicTotal;

            _resultsView.Items.Clear();
            AddRow("Elementos solicitados", elements.ToString("N0"), elements.ToString("N0"));
            AddRow("Elementos almacenados", st.Stored.ToString("N0"), dy.Stored.ToString("N0"));
            AddRow("Elementos rechazados (desbordamiento)", st.Rejected.ToString("N0"), dy.Rejected.ToString("N0"));
            AddRow("Bytes asignados (medidos por el GC)", FormatBytes(st.AllocatedBytes), FormatBytes(dy.AllocatedBytes));
            AddRow("Bytes de datos útiles (almacenados x 4)", FormatBytes(staticUseful), FormatBytes(dynamicUseful));
            AddRow("Bytes desperdiciados - casillas sin usar (estimado)", FormatBytes(staticWasted), "0 B");
            AddRow("Sobrecarga de punteros y encabezados (estimado)", "0 B", FormatBytes(dynamicOverhead));
            AddRow("Eficiencia de memoria (útil / reservado)", $"{staticEfficiency:F1} %", $"{dynamicEfficiency:F1} %");
            AddRow("Tiempo para agregar todos los elementos", $"{st.AddMs:F3} ms", $"{dy.AddMs:F3} ms");
            AddRow("Tiempo para quitar todos los elementos", $"{st.RemoveMs:F3} ms", $"{dy.RemoveMs:F3} ms");

            string summary = staticWasted > dynamicOverhead
                ? $"Con estos valores la estructura estática desperdicia más memoria ({FormatBytes(staticWasted)} en casillas sin usar) " +
                  $"que la que la dinámica gasta en punteros y encabezados ({FormatBytes(dynamicOverhead)})."
                : $"Con estos valores la estructura dinámica gasta más memoria en punteros y encabezados ({FormatBytes(dynamicOverhead)}) " +
                  $"que la que la estática desperdicia en casillas sin usar ({FormatBytes(staticWasted)}).";

            summary += Environment.NewLine + Environment.NewLine +
                $"Punto de equilibrio para int: un nodo dinámico cuesta unas {NodeBytes / DataBytes} veces el dato que guarda, " +
                $"por lo que el arreglo estático es más económico cuando está lleno en más de {100.0 * DataBytes / NodeBytes:F1} %.";

            if (st.Rejected > 0)
                summary += Environment.NewLine + Environment.NewLine +
                    $"La estructura estática rechazó {st.Rejected:N0} elementos porque se excedió su capacidad; " +
                    "la dinámica no tiene ese límite.";

            _summaryBox.Text = summary;
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void AddRow(string metric, string staticValue, string dynamicValue)
    {
        var item = new ListViewItem(metric);
        item.SubItems.Add(staticValue);
        item.SubItems.Add(dynamicValue);
        _resultsView.Items.Add(item);
    }

    private static BenchmarkResult Measure(Func<IDataStructure<int>> factory, int elements)
    {
        long before = GC.GetAllocatedBytesForCurrentThread();
        IDataStructure<int> structure = factory();

        var watch = Stopwatch.StartNew();
        int rejected = 0;
        for (int i = 0; i < elements; i++)
        {
            if (!structure.TryAdd(i))
                rejected++;
        }
        watch.Stop();

        double addMs = watch.Elapsed.TotalMilliseconds;
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        int stored = structure.Count;

        watch.Restart();
        while (structure.TryRemove(out _))
        {
        }
        watch.Stop();

        return new BenchmarkResult(allocated, stored, rejected, addMs, watch.Elapsed.TotalMilliseconds);
    }

    private sealed record BenchmarkResult(long AllocatedBytes, int Stored, int Rejected, double AddMs, double RemoveMs);

    // ------------------------------------------------------------------
    // Reusable tab layout: input bar, visual display and operation log
    // ------------------------------------------------------------------

    private sealed record Command(string Text, Action Handler);

    private sealed class TabUi
    {
        private readonly TextBox _log;

        public TabPage Page { get; }
        public TextBox Input { get; }
        public TextBox Position { get; }
        public TextBox Display { get; }

        public TabUi(string title, bool showPosition, IEnumerable<Command> commands)
        {
            Page = new TabPage(title) { Padding = new Padding(8) };

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 110));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var bar = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };

            bar.Controls.Add(new Label { Text = "Valor:", AutoSize = true, Margin = new Padding(3, 8, 3, 0) });
            Input = new TextBox { Width = 80 };
            bar.Controls.Add(Input);

            Position = new TextBox { Width = 60 };
            if (showPosition)
            {
                bar.Controls.Add(new Label { Text = "Índice:", AutoSize = true, Margin = new Padding(8, 8, 3, 0) });
                bar.Controls.Add(Position);
            }

            foreach (Command command in commands)
            {
                Action handler = command.Handler;
                var button = new Button { Text = command.Text, AutoSize = true };
                button.Click += (_, _) => handler();
                bar.Controls.Add(button);
            }

            Display = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                Font = new Font("Consolas", 11f)
            };

            _log = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            layout.Controls.Add(bar, 0, 0);
            layout.Controls.Add(Display, 0, 1);
            layout.Controls.Add(_log, 0, 2);
            Page.Controls.Add(layout);
        }

        public void Log(string message) =>
            _log.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    }
}

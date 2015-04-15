using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Labs_1_4
{
    public partial class Form1 : Form
    {

        // процессоры
        private readonly List<CPU> _cpuArr = new List<CPU>();

        // номер самого мощного процессора
        private int _maxproc;

        // номер самого слабого процессора
        private int _minproc;

        // количество милисекунд для просчёта
        private int _timecount = 10;

        // время работы над задачами (для пункта 4)
        private int time4;

        // рандомизатор ;)
        private readonly Random _rnd = new Random();

        // список задач
        private readonly List<Process> _pList = new List<Process>();

        public Form1()
        {
            InitializeComponent();
        }

        // запуск симуляции
        private void Button1Click(object sender, EventArgs e)
        {
            Initialization();
        }

        // инициализация стартовых значений симуляции
        private void Initialization()
        {
            CpuInitialization();
            ProcessInitialization();
            StartWork();
        }

        // инициализация процессоров
        private void CpuInitialization()
        {
            _timecount = (int)numericUpDown10.Value;
            _cpuArr.Clear();

            var cpu1Power = (int) numericUpDown5.Value;
            var cpu2Power = (int) numericUpDown6.Value;
            var cpu3Power = (int) numericUpDown7.Value;
            var cpu4Power = (int) numericUpDown8.Value;
            var cpu5Power = (int) numericUpDown9.Value;

            var cpu1 = new CPU(cpu1Power, 20);
            var cpu2 = new CPU(cpu2Power, 20);
            var cpu3 = new CPU(cpu3Power, 20);
            var cpu4 = new CPU(cpu4Power, 20);
            var cpu5 = new CPU(cpu5Power, 20);

            _cpuArr.Add(cpu1);
            _cpuArr.Add(cpu2);
            _cpuArr.Add(cpu3);
            _cpuArr.Add(cpu4);
            _cpuArr.Add(cpu5);


            int max = _cpuArr[0].GetPower();
            int number = 0;
            for (int i = 1; i < 5; i++)
            {
                if (max < _cpuArr[i].GetPower())
                {
                    max = _cpuArr[i].GetPower();
                    number = i;
                }
            }

            _maxproc = number;
            _cpuArr[_maxproc].SetScheduler(true);

            int min = _cpuArr[0].GetPower();
            number = 0;
            for (int i = 1; i < 5; i++)
            {
                if (min > _cpuArr[i].GetPower())
                {
                    min = _cpuArr[i].GetPower();
                    number = i;
                }
            }

            _minproc = number;
        }

        // инициализация списка процессов
        private void ProcessInitialization()
        {
            int counter = 0;
            var chance = (int) numericUpDown1.Value;
            var min = (int) numericUpDown2.Value;
            var max = (int) numericUpDown3.Value;

            if (max < min)
            {
                max = min;
                numericUpDown3.Value = max;
            }

            _pList.Clear();

            /**/richTextBox1.Clear();

            if (radioButton2.Checked) richTextBox1.AppendText("The wickest proccessor: №" + (_minproc + 1).ToString() + "\n");
            if (radioButton3.Checked) richTextBox1.AppendText("Top processor: №" + (_maxproc + 1).ToString() + "\n");
            for (int i = 0; i < _timecount; i++)
            {
                if (GetRandom(chance))
                {

                    var list = new List<int>();
                    for (int j = 0; j < 5; j++)
                    {
                        if (GetRandom(25))
                        {
                            list.Add(j);
                        }
                    }
                    if (radioButton2.Checked)
                    {
                        list.Remove(_minproc);
                    }
                    int k = 0;
                    if (list.Count == 0)
                    {
                        if (radioButton2.Checked)
                        {
                            do
                            {
                                k = _rnd.Next(0, 5);
                            } 
                            while (k == _minproc);
                        }
                        else
                        {
                            k = _rnd.Next(0, 5);
                        }
                        list.Add(k);
                    }
                    var process = new Process(counter+1, i, list, _rnd.Next(min, max));
                    _pList.Add(process);
                    counter++;
                }
            }

        }

        // симуляция работы 
        private void StartWork()
        {
            if (radioButton1.Checked)
            {
                Method1();
            }

            if (radioButton2.Checked)
            {
                Method2();
            }

            if (radioButton3.Checked)
            {
                Method3();
            }

        }

        // поиск наиболее подходящего процессора для заданного процесса
        private int FindCpu(Process p)
        {
            if (p == null) return -1;
            int number = -1;
            int power = 0;
            List<int> numbers = p.GetCpUs();

            foreach(int i in numbers)
            {
                if ((_cpuArr[i].GetPower() > power) && (_cpuArr[i].GetStatus() == CpuStatus.Free))
                {
                    power = _cpuArr[i].GetPower();
                    number = i;
                }
            }
            return number;
        }
        private int FindCpu1(Process p)
        {
            if (p == null) return -1;
            int number = -1;
            int power = 0;
            List<int> numbers = p.GetCpUs();

            foreach(int i in numbers)
            {
                if (_cpuArr[i].GetPower() > power)
                {
                    power = _cpuArr[i].GetPower();
                    number = i;
                }
            }
            return number;
        }

        // FIFO
        private void Method1()
        {

            if (_pList.Count != 0)
            {
                var curproc = _pList[0];

                for (int i = 0; i < _timecount; i++)
                {
                        int number = FindCpu(curproc);
                        while ((curproc != null) && (curproc.GetMsecond() <= i) && (number != -1))
                        {

                            _cpuArr[number].SetProcess(curproc);
                            curproc.SetStatus(Status.Execution);
                            _cpuArr[number].SetStatus(CpuStatus.Busy);
                            _pList.Remove(curproc);
                            if (_pList.Count > 0)
                            {
                                curproc = _pList[0];
                                number = FindCpu(curproc);
                            }
                            else
                            {
                                number = -1;
                                curproc = null;
                            }

                        }

                        foreach (CPU cpu in _cpuArr)
                        {
                            cpu.WorkMethod1();
                        }                   
                }

            }

            richTextBox1.AppendText("Done tasks: " + GetFinishedTasks().ToString() + "\n");
            richTextBox1.AppendText("\n");

            int sumoperations = 0;
            int maxoperations = 0;

            foreach (var c in _cpuArr)
            {
                sumoperations += c.GetOperations();
                maxoperations += c.GetMaxOperations(_timecount);
            }
            richTextBox1.AppendText("Were done for" + _timecount.ToString(CultureInfo.InvariantCulture) + " ms:  " + sumoperations.ToString(CultureInfo.InvariantCulture) + " operations\n");
            richTextBox1.AppendText("Maximum amount of operations for " + _timecount.ToString(CultureInfo.InvariantCulture) + " ms:  " + maxoperations.ToString(CultureInfo.InvariantCulture) + " operations\n");
            double kpd = (double)sumoperations / maxoperations;
            kpd *= 100;
            richTextBox1.AppendText("CoE = CoE' = " + kpd.ToString("f2") + "%\n");

        }

        // с отдельным процессором планировщиком
        private void Method2()
        {
            if (_pList.Count != 0)
            {
                int offset = 0;
                Process curproc = _pList[0];
                for (int i = 0; i < _timecount; i++)
                {
                    offset = 0;
                    while ((FreeCpu() > 0) && (curproc.GetMsecond() <= i) && (offset < _pList.Count))
                    {
                        int number = FindCpu(curproc);
                        if (number != -1)
                        {
                            _cpuArr[number].SetProcess(curproc);
                            curproc.SetStatus(Status.Execution);
                            _cpuArr[number].SetStatus(CpuStatus.Busy);
                            _pList.Remove(curproc);
                        }
                        else
                        {
                            offset++;
                        }
                        if (offset < _pList.Count) curproc = _pList[offset];                        
                    }
                    foreach (var cpu in _cpuArr)
                    {
                        cpu.WorkMethod2();
                    }
                }
            }
            richTextBox1.AppendText("Done tasks: " + GetFinishedTasks().ToString() + "\n");
            richTextBox1.AppendText("\n");

            int sumoperations = 0;
            int maxoperations = 0;

            foreach (var c in _cpuArr)
            {
                sumoperations += c.GetOperations();
                maxoperations += c.GetMaxOperations(_timecount);
            }
            richTextBox1.AppendText("Were done for " + _timecount.ToString(CultureInfo.InvariantCulture) + " ms:  " + sumoperations.ToString(CultureInfo.InvariantCulture) + " operations\n");
            richTextBox1.AppendText("Maximum amount of operations for " + _timecount.ToString(CultureInfo.InvariantCulture) + " ms:  " + maxoperations.ToString(CultureInfo.InvariantCulture) + " operations\n");
            double kpd = (double)sumoperations / maxoperations;
            kpd *= 100;
            richTextBox1.AppendText("CoE = " + kpd.ToString("f2") + "%\n");

            double kpd_ = (double)sumoperations /( maxoperations - _cpuArr[_minproc].GetMaxOperations(_timecount));
            kpd_ *= 100;
            richTextBox1.AppendText("CoE' = " + kpd_.ToString("f2") + "%\n");


        }

        // возложение функции планирования на самый мощный процессор
        private void Method3()
        {

            int worktime = 20;

            int nextPlan = worktime + 4;
            int nextWork = 4;
            //счётчик планирований
            int counter = 1;


            if (_pList.Count != 0)
            {
                Process curproc = _pList[0];
                for (int i = 0; i < _timecount; i++)
                {
                    if (i < nextWork)
                    {
                        int start = i;
                        while ((_pList.Count != 0) && (curproc.GetMsecond() < (start + worktime)))
                        {
                            curproc = _pList[0];

                            int number = FindCpu(curproc);
                            if (number == -1)
                            {
                                number = FindCpu1(curproc);
                            }

                            _cpuArr[number].AddProcess(curproc);
                            _cpuArr[number].SetStatus(CpuStatus.Busy);

                            _pList.Remove(curproc);
                            if (_pList.Count != 0) curproc = _pList[0];
                            else curproc = null;
                        }
                        for (int j = 0; j < 5; j++)
                        {
                            if (j != _maxproc)
                            {
                                _cpuArr[j].WorkMethod3();
                            }
                            else
                            {
                                _cpuArr[j].IncreaseCurrenttime();
                            }
                        }

                    }

                    if ((i < nextPlan) && (i >= nextWork))
                    {
                        foreach (CPU c in _cpuArr)
                        {
                            c.WorkMethod3();
                        }
                    }

                    if (i == nextPlan)
                    {
                        nextWork += worktime + 4;
                        nextPlan += worktime + 4;
                        counter++;
                    }
                }
            }


            richTextBox1.AppendText("Done tasks: " + GetFinishedTasks().ToString() + "\n");
            richTextBox1.AppendText("\n");

            int sumoperations = 0;
            int maxoperations = 0;

            foreach (var c in _cpuArr)
            {
                sumoperations += c.GetOperations();
                maxoperations += c.GetMaxOperations(_timecount);
            }
            richTextBox1.AppendText("Were done for " + _timecount.ToString(CultureInfo.InvariantCulture) + " ms:  " + sumoperations.ToString(CultureInfo.InvariantCulture) + " operations\n");
            richTextBox1.AppendText("Maximum amount of operations for " + _timecount.ToString(CultureInfo.InvariantCulture) + " ms:  " + maxoperations.ToString(CultureInfo.InvariantCulture) + " operations\n");
            
            double kpd = (double)sumoperations / maxoperations;
            kpd *= 100;
            richTextBox1.AppendText("CoE = " + kpd.ToString("f2") + "%\n");

            maxoperations -= _cpuArr[_maxproc].GetPower()*counter*4;
            double kpd_ = (double)sumoperations / maxoperations;
            kpd_ *= 100;
            richTextBox1.AppendText("CoE' = " + kpd_.ToString("f2") + "%\n");


            
        }

        // возврат рандомного true или false по заданной вероятности
        private bool GetRandom(int chance)
        {
            int value = _rnd.Next(1, 100);
            return (value <= chance);
        }

        // изменение времени работы системы
        private void NumericUpDown10ValueChanged(object sender, EventArgs e)
        {
            _timecount = (int) numericUpDown10.Value;
        }

        // подсчёт количества выполненых задач
        private int GetFinishedTasks()
        {
            int sum = 0;
            foreach (var cpu in _cpuArr)
            {
                sum += cpu.GetFinishedTasks();
            }
            return sum;
        }

        // возвращает количество свободных в даный момент процессоров
        private int FreeCpu()
        {
            int ret = 0;
            foreach (var CPU in _cpuArr)
            {
                if (CPU.GetStatus() == CpuStatus.Free)
                {
                    ret++;
                }
            }
            ret--; // планировщик в вычислениях не участвует
            return ret;
        }

    }

    // реализация процессора
    public class CPU
    {
        // процессор планирования
        private bool _scheduler = false;
        // статус занятости
        private CpuStatus _st;
        // мощность процессора
        private int _power;
        // текущая задача
        private Process _process;
        // время работы над задачами (для пункта 4)
        private int _worktime;
        // текущее время работы в миллисекундах
        private int _currenttime;
        // время простоя
        private int _downtime;
        // список выполненных процессором задач
        private List<Process> _finishedProcesses;
        // список ожидающих задач
        private List<Process> _waitingProcesses;
        // конструктор
        public CPU(int power, int worktime)
        {
            _power = power;
            _worktime = worktime;
            _finishedProcesses = new List<Process>();
            _waitingProcesses = new List<Process>();
            _currenttime = 0;
            _downtime = 0;
            _st = CpuStatus.Free; 
        }
        // установка статуса процессора
        public void SetStatus(CpuStatus st)
        {
            _st = st;
        }
        // возврат статуса процессора
        public CpuStatus GetStatus()
        {
            return _st;
        }
        // установка мощности процессора
        public void SetPower(int power)
        {
            _power = power;
        }
        // возврат мощности процессора
        public int GetPower()
        {
            return _power;
        }
        // установка текущей задачи на процессоре
        public void SetProcess(Process process)
        {
            _process = process;
        }
        // возврат текущей задачи
        public Process GetProcess()
        {
            return _process;
        }
        // установка времени работы над задачами
        public void SetWorktime(int worktime)
        {
            _worktime = worktime;
        }
        // возврат времени работы над задачами
        public int GetWorktime()
        {
            return _worktime;
        }
        // возврат количества выполненых операций за заданное время
        public int GetOperations()
        {
            int Sum = 0;
            foreach (var p in _finishedProcesses)
            {
                Sum += p.GetOperations();
            }
            if (_process != null)
            {
                Sum += _process.GetCurOperation();
            }
            return Sum;
        }
        // возврат максимального количества операций за заданое время
        public int GetMaxOperations(int time)
        {
            return time * _power;
        }
        // возврат количества выполненых задач
        public int GetFinishedTasks()
        {
            return _finishedProcesses.Count;
        }
        // увеличение текущего времени
        public void IncreaseCurrenttime()
        {
            _currenttime++;
        }
        // увеличение времени простоя
        public void IncreaseDowntime()
        {
            _downtime++;
        }
        // 
        public void SetScheduler(bool b)
        {
            _scheduler = b;
        }
        //
        public bool GetScheduler()
        {
            return _scheduler;
        }
        // добавить задачу в очередь ожидания
        public void AddProcess(Process process)
        {
            _waitingProcesses.Add(process);
        }
        // такт работы по пункту 1
        public void WorkMethod1()
        {
            if (_process != null)
            {
                _process.IncreaseCurrentOperation(_power);
                Status st = _process.GetStatus();
                if (st == Status.Finished)
                {
                    _process.SetFinishTime(_currenttime);
                    _finishedProcesses.Add(_process);
                    _process = null;
                    SetStatus(CpuStatus.Free);
                }
            }
            else
            {
                IncreaseDowntime();
            }
            IncreaseCurrenttime();
        }
        // такт работы по пункту 2
        public void WorkMethod2()
        {
            if (_process != null)
            {
                _process.IncreaseCurrentOperation(_power);
                Status st = _process.GetStatus();
                if (st == Status.Finished)
                {
                    _process.SetFinishTime(_currenttime);
                    _finishedProcesses.Add(_process);
                    _process = null;
                    _st = CpuStatus.Free;
                }
            }
            else
            {
                IncreaseDowntime();
            }
            IncreaseCurrenttime();
        }
        // такт работы по пункту 3
        public void WorkMethod3()
        {
            if (_process == null)
            {
                if (_waitingProcesses.Count == 0)
                {
                    IncreaseDowntime();
                    IncreaseCurrenttime();
                    return;
                }
                Process cur = _waitingProcesses[0];
                _process = cur;
                _process.SetStatus(Status.Execution);
                _waitingProcesses.Remove(cur);
            }

            if (_process == null)
            {
                IncreaseDowntime();
            }
            else
            {
                _process.IncreaseCurrentOperation(_power);
                Status st = _process.GetStatus();
                if (st == Status.Finished)
                {
                    _process.SetFinishTime(_currenttime);
                    _finishedProcesses.Add(_process);
                    _process = null;
                    if (_waitingProcesses.Count == 0) _st = CpuStatus.Free;
                }
               
            }

            IncreaseCurrenttime();
        }
    }

    // реализация задачи
    public class Process
    {
        // номер процесса
        private readonly int _number;
        // время возникновения процесса
        private readonly int _msecond;
        // статус
        private Status _st;
        // список процессоров на которых может выполниться задача
        private readonly List<int> _list = new List<int>();
        // количество операций
        private readonly int _operations;
        // текущая операция
        private int _currentOperation;
        // номер процессора на котором выполняется задача
        private int _procnumber;
        // время начала выполнения
        private int _starttime;
        // время окончания выполнения
        private int _finishtime;
        // конструктор
        public Process(int number, int msecond, List<int> list, int operations)
        {
            _number = number;
            _msecond = msecond;
            _st = Status.Waiting;
            _list = list;
            _operations = operations;
            _currentOperation = 0;
        }
        // возможность выполнения на указанном процессоре: 1 - 5
        public bool CanStart(int number)
        {
            return _list.Contains(number);
        }
        // увеличение текущей операции
        public void IncreaseCurrentOperation(int value)
        {
            _currentOperation += value;
            if (_currentOperation >= _operations)
            {
                _st = Status.Finished;
            }
        }
        // старт выполнения
        public void Start()
        {
            _st = Status.Execution;
        }
        // установка статуса
        public void SetStatus(Status status)
        {
            _st = status;
        }
        // возврат статуса
        public Status GetStatus()
        {
            return _st;
        }
        // установка процессора 
        public void SetProcNumber(int procnumber)
        {
            _procnumber = procnumber;
        }
        // возврат номера процессора
        public int GetProcNumber()
        {
            return _procnumber;
        }
        // возврат номера процесса
        public int GetNumber()
        {
            return _number;
        }
        // возврат секунды возникновения
        public int GetMsecond()
        {
            return _msecond;
        }
        // установка времени начала выполнения
        public void SetStartTime(int time)
        {
            _starttime = time;
        }
        // установка времени окончания выполнения
        public void SetFinishTime(int time)
        {
            _finishtime = time;
        }
        // возврат времени начала выполнения
        public int GetStartTime()
        {
            return _starttime;
        }
        // возврат времени окончания выполнения
        public int GetFinishTime()
        {
            return _finishtime;
        }
        // печать доступных процессоров для задачи
        public string CpuPrint()
        {
            return _list.Aggregate("", (current, k) => current + ("<<CPU" + (k + 1).ToString(CultureInfo.InvariantCulture) + ">> "));
        }
        // возврат списка доступных процессоров для выполнения
        public List<int> GetCpUs()
        {
            return _list;
        }
        // возврат количества операций
        public int GetOperations()
        {
            return _operations;
        }
        // возврат текущей операции
        public int GetCurOperation()
        {
            return _currentOperation;
        }
    }

    // статус выполнения задачи
    public enum Status
    {
        // ожидание выполнения
        Waiting, 
        // выполнение
        Execution, 
        // выполнение завершено
        Finished
    }

    // статус процессора
    public  enum CpuStatus
    {
        // занят
        Busy,
        // свободен
        Free
    }
}

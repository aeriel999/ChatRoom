﻿using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CliientApp
{
    /// <summary>
    /// Interaction logic for Private_Chate.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class Private_Chate : Window
    {
        public string Login { get; set; }
        public Private_Chate(string login)
        {
            InitializeComponent();

            Login = login;

            this.DataContext = this;
        }
    }
}

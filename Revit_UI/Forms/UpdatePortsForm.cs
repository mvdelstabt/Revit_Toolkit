/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BH.UI.Revit.Forms
{
    public partial class UpdatePortsForm : Form
    {
        public UpdatePortsForm()
        {
            InitializeComponent();
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            RevitListener listener = RevitListener.Listener;

            int inPort = (int)Math.Round(InputPort.Value);
            int outPort = (int)Math.Round(OutputPort.Value);

            if (inPort == outPort || inPort < 3000 || inPort > 65000 || outPort < 3000 || outPort > 65000)
            {
                MessageBox.Show("Input port and output port must have values between 3000 and 65000 and can not be the same", "Port number error");
                return;
            }

            listener.SetPorts(inPort, outPort);
            this.Close();

        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P520233_AllanDelgado.Formularios
{
    public partial class FrmUsuariosGestion : Form
    {
        //objeto local de tipo usuario. 
        private Logica.Models.Usuario MiUsuarioLocal { get; set; }

        public FrmUsuariosGestion()
        {
            InitializeComponent();

            MiUsuarioLocal = new Logica.Models.Usuario();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FrmUsuariosGestion_Load(object sender, EventArgs e)
        {
            MdiParent = Globales.ObjetosGlobales.MiFormularioPrincipal;

            CargarComboRolesDeUsuario();

            CargarListaUsuarios();

        }

        private void CargarComboRolesDeUsuario()
        {
            Logica.Models.UsuarioRol MiRol = new Logica.Models.UsuarioRol();

            DataTable dt = new DataTable();

            dt = MiRol.Listar();

            if (dt != null && dt.Rows.Count > 0)
            {
                //una asegurado que el dt tiene valores, los "dibujo" en el combobox
                CboxUsuarioTipoRol.ValueMember = "id";
                CboxUsuarioTipoRol.DisplayMember = "Descripcion";

                CboxUsuarioTipoRol.DataSource = dt;

                CboxUsuarioTipoRol.SelectedIndex = -1;

            }

        }



        //todas las funcionalidades especificas y que se puedan reutilizar DEBEN 
        //ser encapsuladas 
        private void CargarListaUsuarios()
        {
            Logica.Models.Usuario miusuario = new Logica.Models.Usuario();

            DataTable lista = new DataTable();

            lista = miusuario.ListarActivos();

            DgvListaUsuarios.DataSource = lista;
        }

        private bool ValidarDatosRequeridos()
        {
            bool R = false;

            //validar que se hayan digitado valores en los campos obligatorios
            if (!string.IsNullOrEmpty(TxtUsuarioCedula.Text.Trim()) &&
                !string.IsNullOrEmpty(TxtUsuarioNombre.Text.Trim()) &&
                !string.IsNullOrEmpty(TxtUsuarioCorreo.Text.Trim()) &&
                !string.IsNullOrEmpty(TxtUsuarioContrasennia.Text.Trim()) &&
                CboxUsuarioTipoRol.SelectedIndex > -1
                )
            {
                R = true;
            }
            else
            {
                //indicar al usuario qué validación está faltando

                //CEDULA
                if (string.IsNullOrEmpty(TxtUsuarioCedula.Text.Trim()))
                {
                    MessageBox.Show("Debe digitar la Cédula", "Error de validación", MessageBoxButtons.OK);
                    return false;
                }

                //NOMBRE
                if (string.IsNullOrEmpty(TxtUsuarioNombre.Text.Trim()))
                {
                    MessageBox.Show("Debe digitar el Nombre", "Error de validación", MessageBoxButtons.OK);
                    return false;
                }

                //CORREO
                if (string.IsNullOrEmpty(TxtUsuarioCorreo.Text.Trim()))
                {
                    MessageBox.Show("Debe digitar el Correo", "Error de validación", MessageBoxButtons.OK);
                    return false;
                }

                //CONTRASEÑA
                if (string.IsNullOrEmpty(TxtUsuarioContrasennia.Text.Trim()))
                {
                    MessageBox.Show("Debe digitar la Contraseña", "Error de validación", MessageBoxButtons.OK);
                    return false;
                }

                //ROL DE USUARIO
                if (CboxUsuarioTipoRol.SelectedIndex == -1)
                {
                    MessageBox.Show("Debe Seleccionar un Rol de Usuario", "Error de validación", MessageBoxButtons.OK);
                    return false;
                }

            }

            return R;
        }


        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            //lo primero que debemos hacer es validar los datos mínimos requeridos, 
            //esto se hace para evitar que queden registros sin datos a nivel de db
            //pero también porque se un campo de base de datos no acepta valores NULL
            //y se llama al INSERT, dará un error. 

            //Luego de esto y tomando en consideración el diagrama de casos de uso expandido
            //de usuario, hay que hacer validar que NO exista un usuario con la cedula y/o 
            //correo que se digitaron. (No se pueden repetir estos datos en distintas
            //filas en la tabla Usuario

            //Si ambas validaciones son Negativas entonces se procede a Agregar() el usuario.

            //---------------------------------//

            //usaremos un objeto local de tipo Usuario, que será al que daremos forma para luego
            //usar las funciones como agregar, actualizar, eliminar, etc. 

            if (ValidarDatosRequeridos())
            {
                MiUsuarioLocal = new Logica.Models.Usuario();

                MiUsuarioLocal.Cedula = TxtUsuarioCedula.Text.Trim();
                MiUsuarioLocal.Nombre = TxtUsuarioNombre.Text.Trim();
                MiUsuarioLocal.Correo = TxtUsuarioCorreo.Text.Trim();
                MiUsuarioLocal.Telefono = TxtUsuarioTelefono.Text.Trim();

                //con el combo de rol hay que extraer el valuemember seleccionado. 
                MiUsuarioLocal.MiUsuarioRol.UsuarioRolID = Convert.ToInt32(CboxUsuarioTipoRol.SelectedValue);

                MiUsuarioLocal.Contrasennia = TxtUsuarioContrasennia.Text.Trim();
                MiUsuarioLocal.Direccion = TxtUsuarioDireccion.Text.Trim();

                bool CedulaOk = MiUsuarioLocal.ConsultarPorCedula(MiUsuarioLocal.Cedula);

                bool CorreoOk = MiUsuarioLocal.ConsultarPorCorreo(MiUsuarioLocal.Correo);

                if (CedulaOk == false && CorreoOk == false)
                {
                    //se solicita confirmación por parte del usuario 
                    string Pregunta = string.Format("¿Está seguro de agregar al usuario {0}?", MiUsuarioLocal.Nombre);
                    DialogResult respuesta = MessageBox.Show(Pregunta, "???", MessageBoxButtons.YesNo);

                    if (respuesta == DialogResult.Yes)
                    {
                        //procedemos a Agregar el usuario
                        bool ok = MiUsuarioLocal.Agregar();

                        if (ok)
                        {
                            MessageBox.Show("Usuario ingresado correctamente!", ":)", MessageBoxButtons.OK);

                            LimpiarForm();
                            CargarListaUsuarios();

                        }
                        else
                        {
                            MessageBox.Show("El Usuario no se pudo agregar...", ":(", MessageBoxButtons.OK);
                        }
                    }
                }
            }
        }


        private void LimpiarForm()
        {
            TxtUsuarioCodigo.Clear();
            TxtUsuarioCedula.Clear();
            TxtUsuarioNombre.Clear();
            TxtUsuarioCorreo.Clear();
            TxtUsuarioTelefono.Clear();
            TxtUsuarioContrasennia.Clear();
            TxtUsuarioDireccion.Clear();

            CboxUsuarioTipoRol.SelectedIndex = -1;

            CbUsuarioActivo.Checked = false;

        }






    }
}

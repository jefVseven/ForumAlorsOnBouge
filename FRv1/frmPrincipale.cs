﻿using ClassesMetiers;
using DAO;
using Methodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IHMFR
{
    public partial class frmPrincipale : Form
    {
        static internal int IsRmodo;
        static internal Utilisateur CurrentUsers;
        static internal bool IsConnected;

        public frmPrincipale()
        {
            InitializeComponent();
        }

        #region Load

        private void frmPrincipale_Load(object sender, EventArgs e)
        {
            displaycbBxRubric(RubricDAO.GetAllRubrics());
            displaycbBxSubject(SubjectDAO.GetSujetsByCategorieID((int)cbBxRubric.SelectedValue));
            visibiliteModerateur();
        }

        private void displaycbBxRubric(List<Rubric> rubrics)
        {
            cbBxRubric.ValueMember = "ID_RUBRIC";
            cbBxRubric.DisplayMember = "RUBRIC_TITLE";
            cbBxRubric.DataSource = rubrics;
        }

        private void displaycbBxSubject(List<Subject> subjects)
        {
            cbBxSubject.ValueMember = "ID_SUBJECT";
            cbBxSubject.DisplayMember = "SUBJECT_TITLE";
            cbBxSubject.DataSource = subjects;
            Subject subject = (Subject)cbBxSubject.SelectedItem;
        }

        private void visibiliteModerateur()
        {
            tbLPModerator.Visible = frmPrincipale.IsConnected;
            tbLPUserLogged.Visible = frmPrincipale.IsConnected;
            btConnect.Visible = frmPrincipale.IsConnected;
            //gpBoxAdministration.Visible = FrmAccueil.IsRmodo;
            //btLogin.Visible = !FrmAccueil.IsConnected;
        }

        #endregion

        #region buttons click

        private void btAnnuler_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btConnect_Click(object sender, EventArgs e)
        {
            using (Identification frmIdentity = new Identification())
            {
                frmIdentity.ShowDialog();
            }
        }

        private void btCreerSujet_Click(object sender, EventArgs e)
        {
            using (frmCreerSujet creerSujet = new frmCreerSujet())
            {
                creerSujet.rubric = (Rubric)cbBxRubric.SelectedItem;
                creerSujet.Text = string.Format("Ajouter le sujet dans la rubrique {0}", creerSujet.rubric.rubric_title);
                creerSujet.ShowDialog();
                displaycbBxSubject(SubjectDAO.GetSujetsByCategorieID((int)cbBxRubric.SelectedValue));
            }
        }

        private void btCreerPost_Click(object sender, EventArgs e)
        {
            using (CreerPost frmCreerPost = new CreerPost())
            {
                frmCreerPost.post = (Post)cbBxSubject.SelectedItem;
                frmCreerPost.Text = string.Format("Ajout du post au sujet {0}", frmCreerPost.post.subject_title);
                frmCreerPost.ShowDialog();
                displaydgVPost(PostDAO.GetAllPostBySubject((int)cbBxSubject.SelectedValue));
            }
        }

        private void displaydgVPost(List<Post> posts)
        {
            dGVPost.DataSource = posts;
            dGVPost.Columns["ID"].Visible = false;
            dGVPost.Columns["SUJET"].Visible = false;
            dGVPost.Columns["UTILISATEUR"].Visible = false;
        }

        #endregion

        #region modérateur

        private void btModifSujet_Click(object sender, EventArgs e)
        {
            using (frmModifSujet modifSujet = new frmModifSujet())
            {
                modifSujet.rubric = (Rubric)cbBxRubric.SelectedItem;
                modifSujet.subject = (Subject)cbBxSubject.SelectedItem;
                modifSujet.Text = string.Format("Modifie le sujet {0} dans la rubrique {1}", modifSujet.subject.subject_title, modifSujet.subject.subject_description);
                modifSujet.ShowDialog();
                displaycbBxSubject(SubjectDAO.GetSujetsByCategorieID((int)cbBxRubric.SelectedValue));
            }
        }

        private void btSupSujet_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            dr = MessageBox.Show(Properties.Resources.MsgBoxDeleteSujetText, Properties.Resources.MsgBoxDeleteSujetTitre, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (dr == DialogResult.OK)
            {
                if (Controller.DeleteSujet((int)cbBxSubject.SelectedValue) != 1)
                {
                    MessageBox.Show(Properties.Resources.MsgBoxErreurDeleteSujetText, Properties.Resources.MsgBoxErreurDeleteSujetTitre);

                }
                List<Subject> subjects = SubjectDAO.GetSujetsByCategorieID((int)cbBxRubric.SelectedValue);
                if (subjects != null)
                {
                    displaycbBxSubject(subjects);
                }
            }
            if (visibiliteSujets())
            {
                PanelSujetVisible();
            }
            else
            {
                PanelSujetInvisible();
            }
        }

        private bool visibiliteSujets()
        {
            if (cbBxRubric.SelectedIndex != -1)
            {
                if (Controller.GetSujetsByCategorieID((int)cbBxRubric.SelectedValue) != null)
                {
                    return true;
                }
            }
            return false;
        }

        private void PanelSujetVisible()
        {
            cbBxSubject.Visible = true;
            gpBxDescSubject.Visible = true;
            lbPasDeSujet.Visible = false;
            btModifSujet.Enabled = true;
            btSupSujet.Enabled = true;
        }

        private void PanelSujetInvisible()
        {
            cbBxSubject.Visible = false;
            gpBxDescSubject.Visible = false;
            lbPasDeSujet.Visible = true;
            btModifSujet.Enabled = false;
            btSupSujet.Enabled = false;
        }

        private void btSupPost_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            dr = MessageBox.Show(Properties.Resources.MsgBoxDeletePostText, Properties.Resources.MsgBoxDeletePostTitre, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (dr == DialogResult.OK)
            {
                if (PostDAO.DeleteReponse((int)dGVPost.CurrentRow.Cells["ID_POST"].Value) != 1)
                {
                    MessageBox.Show(Properties.Resources.MsgBoxErreurDeletePostText, Properties.Resources.MsgBoxErreurDeletePostTitre);
                }
                List<Post> posts = PostDAO.GetAllPostBySubject((int)cbBxSubject.SelectedValue);
                if (posts != null)
                {
                    displaydgVPost(posts);
                }
                if (visibilitePost())
                {
                    PanelSujetInvisible();
                }
                else
                {
                    PanelSujetInvisible();
                }
            }
        }

        private bool visibilitePost()
        {
            if (cbBxSubject.SelectedIndex != -1)
            {
                if (PostDAO.GetAllPostBySubject((int)cbBxSubject.SelectedValue) != null)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region event indexchanged

        private void cbBxRubric_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Subject> subjects = SubjectDAO.GetSujetsByCategorieID((int)cbBxRubric.SelectedValue);
            if (subjects != null)
            {
                PanelSujetVisible();
                displaycbBxSubject(subjects);
            }
            else
            {
                PanelSujetInvisible();
            }
        }

        private void cbBxSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (visibiliteSujets())
            {
                Subject subject = (Subject)cbBxSubject.SelectedItem;
                txtDescSujet.Text = subject.subject_description;

                if (visibiliteSujets())
                {
                    PanelPostVisible();
                    List<Post> posts = PostDAO.GetAllPostBySubject((int)cbBxSubject.SelectedValue);
                    if (posts != null)
                    {
                        displaydgVPost(posts);
                    }
                }
                else
                {
                    PanelPostInvisible();
                }
            }
            else
            {
                PanelSujetInvisible();
            }
        }

        private void PanelPostVisible()
        {
            dGVPost.Visible = true;
            lbPasDePost.Visible = false;
            btSupPost.Enabled = true;
        }

        private void PanelPostInvisible()
        {
            dGVPost.Visible = false;
            lbPasDePost.Visible = true;
            btSupPost.Enabled = false;
        }

        private void dGVPost_SelectionChanged(object sender, EventArgs e)
        {
            if(visibilitePost())
            {
                PanelPostVisible();
            }
            else
            {
                PanelPostInvisible();
            }
        }

        #endregion


    }
}

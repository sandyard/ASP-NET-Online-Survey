﻿using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

public partial class Admin_PageLogUser : PageBase
{
    static string repertoireLog = "~/Logs/";

    protected void Page_Load( object sender, EventArgs e )
    {
        if ( !Page.IsPostBack )
        {
            LabelErreurMessage.Text = "";

            BindFichiers();
            TailleDesLogs();
        }
    }

    // Calculer la taille de tous les fichiers dans repertoireLog
    protected void TailleDesLogs()
    {
        string directory = Server.MapPath( VirtualPathUtility.ToAbsolute( repertoireLog ) );
        long size = RepertoireInfo.GetTaille( directory );
        LabelTailleUserFiles.Text = "Taille des Logs : " + Tools.FileSizeFormat( size, "N" );
    }

    protected void BindFichiers()
    {
        string dirName = Server.MapPath( repertoireLog );
        TextBoxLog.Text = "";

        if ( Directory.Exists( dirName ) )
        {
            List<Fichier> fichiers = GetAllFichiers( dirName );
            LabelMessage.Text = "Visiteurs différents : " + fichiers.Count.ToString();
            foreach ( Fichier f in fichiers )
            {
                string logs = "";
                FileStream fs = new FileStream( f.Nom, FileMode.Open, FileAccess.Read );
                StreamReader sr = new StreamReader( fs );
                logs = sr.ReadToEnd();
                sr.Close();
                fs.Close();

                string fichier = "";
                string[] nomFichier = f.Nom.Split( '\\' );
                string nom = nomFichier[ nomFichier.Length - 1 ];
                fichier += "\n------ " + nom + "\n";
                fichier += logs;
                TextBoxLog.Text += fichier;
            }
        }
    }

    protected List<Fichier> GetAllFichiers( string dirName )
    {
        List<Fichier> list = new List<Fichier>();
        string[] fichiers = Directory.GetFiles( dirName );

        foreach ( string f in fichiers )
        {
            Fichier curr = new Fichier
            (
                f,
                File.GetLastWriteTime( f )
            );

            list.Add( curr );
        }
        IComparer<Fichier> c = new Fichier();
        list.Sort( c );
        list.Reverse();
        return list;
    }

    protected void ButtonSupprimer_Click( object sender, EventArgs e )
    {
        string dirName = Server.MapPath( repertoireLog );

        if ( TextBoxFichier.Text.Trim() == "" )
        {
            LabelErreurMessage.Visible = true;
            LabelErreurMessage.CssClass = "LabelValidationMessageErrorStyle";
            LabelErreurMessage.Text = "Donner un fichier à supprimer.";
            return;
        }

        string file = TextBoxFichier.Text.Trim();
        file = dirName + file;
        if ( ! File.Exists( file ) )
        {
            LabelErreurMessage.Visible = true;
            LabelErreurMessage.CssClass = "LabelValidationMessageErrorStyle";
            LabelErreurMessage.Text = "Le fichier n'existe pas.";
            return;
        }

        File.Delete( file );
        LabelErreurMessage.Visible = true;
        LabelErreurMessage.CssClass = "InformationMessageStyle";
        LabelErreurMessage.Text = "Fichier supprimé avec succès.";

        BindFichiers();
        TailleDesLogs();
    }

    protected void ButtonEffacer_Click( object sender, EventArgs e )
    {
        string dirName = Server.MapPath( repertoireLog );

        if ( Directory.Exists( dirName ) )
        {
            List<Fichier> fichiers = GetAllFichiers( dirName );
            foreach ( Fichier fi in fichiers )
            {
                File.Delete( fi.Nom );
            }
            BindFichiers();
            TailleDesLogs();
        }
    }
}

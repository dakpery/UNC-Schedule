<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="UNC_Schedule._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">



    <link href="Content/myCSS.css" rel="stylesheet" />
    <script type="text/javascript">
        function callAjax(e) {
            e.preventDefault();

            $.ajax
            ({

             type: "POST",
             url: "Default.aspx/backEndFunction",
             data: "{}",
             contentType: "application/json; charset=utf-8",
             dataType: "json",
              success: function (response) {
                 alert(response.d);
              },
              failure: function(response) {
                 alert("broken")
              }


            })
        }
        
    </script>

    <script src="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.3.1.min.js"></script>
    <asp:Button ID="Button1" runat="server"  onclick="Button1_Click" Text="Button" /> 
    
    <asp:Button ID="ajaxbtn" runat="server"  onClientClick="callAjax(event)" Text="Ajax" />  
</asp:Content>

<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LyricDownloadWebApp.Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <br/>
    <asp:CheckBox runat="server" Text="Use async method to manage donwload tasks" ID="cbxUseAsyncMethod"/><br/>
    <asp:CheckBox runat="server" Text="Use async method to download songs" ID="cbxUseAsyncSongs"/><br/>
    <asp:CheckBox runat="server" Text="Use async FileWriter" ID="cbxUseAsyncFileWriter"/>
    
    
    
    <br/><br/>
    <div>
        
        <asp:CheckBoxList ID="cbxSongs" runat="server">
        </asp:CheckBoxList>
        <hr/>
        <asp:Button ID="btnSubmit" OnClick="btnSubmit_Click" Text="Download Selected Songs" runat="server"/>
    </div>
    <asp:Literal ID="litOutput" runat="server"></asp:Literal>
    <%--<asp:Label ID="lblOutput" Text="" runat="server"></asp:Label>--%>

</asp:Content>

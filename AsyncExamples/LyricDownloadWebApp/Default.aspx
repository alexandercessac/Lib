<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LyricDownloadWebApp.Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <br/>
    <asp:CheckBox runat="server" Text="Use await/async" ID="cbxAsync"/><br/>
    <asp:CheckBox runat="server" Text="Use await in FileWriter" ID="cbxAsyncFileWriter"/>
    <br/><br/>
    <div>
        &nbsp;&nbsp;&nbsp;&nbsp;<b>Drake</b><br/>
        <asp:CheckBoxList ID="cbxSongs" runat="server">
        </asp:CheckBoxList>
        <hr/>
        <asp:Button ID="btnSubmit" OnClick="btnSubmit_Click" Text="Download Selected Songs" runat="server"/>
    </div>
    <asp:Literal ID="litOutput" runat="server"></asp:Literal>
    <%--<asp:Label ID="lblOutput" Text="" runat="server"></asp:Label>--%>

</asp:Content>

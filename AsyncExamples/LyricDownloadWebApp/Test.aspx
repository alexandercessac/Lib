<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="LyricDownloadWebApp.Test" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Button runat="server" Text="RunTest" ID="btnTest" OnClick="btnTest_OnClick"/>
    <asp:Label runat="server" ID="lblResult"></asp:Label>
</asp:Content>

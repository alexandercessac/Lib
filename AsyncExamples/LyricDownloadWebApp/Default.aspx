<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LyricDownloadWebApp._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <br/>
    <asp:CheckBox runat="server" Text="Use await/async" ID="cbxAsync"/>
    <br/><br/>
    <div>
        &nbsp;&nbsp;&nbsp;&nbsp;<b>Drake</b><br/>
        <asp:CheckBoxList ID="cbxDrakeSongs" runat="server">
            <Items>
                <asp:ListItem Text="hotline-bling" Value="drake-hotline-bling"></asp:ListItem>
                <asp:ListItem Text="all-me" Value ="drake-all-me"></asp:ListItem>
                <asp:ListItem Text="forever" Value="drake-forever"></asp:ListItem>
            </Items>
        </asp:CheckBoxList>
        <hr/>
        &nbsp;&nbsp;&nbsp;&nbsp;<b>2 Chainz</b><br/>
        <asp:CheckBoxList runat="server" ID="cbxTwoChainzSongs">
            <Items>
                <asp:ListItem Text="im-different" Value="2-chainz-im-different"></asp:ListItem>
                <asp:ListItem Text="no-lie" Value ="2-chainz-no-lie"></asp:ListItem>
                <asp:ListItem Text="feds-watching" Value="2-chainz-feds-watching"></asp:ListItem>
            </Items>
        </asp:CheckBoxList>
        <hr/>
        &nbsp;&nbsp;&nbsp;&nbsp;<b>Lil Wayne</b><br/>
        <asp:CheckBoxList ID="cbxLilWayneSongs" runat="server" >
            <Items>
                <asp:ListItem Text="6-foot-7-foot" Value="Lil-Wayne-6-foot-7-foot"></asp:ListItem>
                <asp:ListItem Text="love-me" Value ="Lil-Wayne-love-me"></asp:ListItem>
                <asp:ListItem Text="lollipop" Value="Lil-Wayne-lollipop"></asp:ListItem>
            </Items>
        </asp:CheckBoxList>
        <hr/>
        <asp:Button ID="btnSubmit" OnClick="btnSubmit_Click" Text="Download Selected Songs" runat="server"/>
    </div>
    <asp:Literal ID="litOutput" runat="server"></asp:Literal>
    <%--<asp:Label ID="lblOutput" Text="" runat="server"></asp:Label>--%>

</asp:Content>

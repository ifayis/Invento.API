namespace Invento.Application.Common
{
    public static class EmailTemplates
    {
        public static string TemporaryPassword(
            string fullName,
            string companyName,
            string email,
            string password,
            string role)
        {
            return $@"
                <h2>Welcome to {companyName}</h2>

                <p>Hello <b>{fullName}</b>,</p>

                <p>
                    Your Invento account has been created.
                </p>

                <table style='border-collapse:collapse'>
                    <tr>
                        <td><b>Email</b></td>
                        <td>{email}</td>
                    </tr>
                    <tr>
                        <td><b>Temporary Password</b></td>
                        <td>{password}</td>
                    </tr>
                    <tr>
                        <td><b>Role</b></td>
                        <td>{role}</td>
                    </tr>
                </table>

                <br/>

                <p>
                    You must change your password immediately after your first login.
                </p>

                <br/>

                <p>
                    Regards,<br/>
                    Invento ERP
                </p>";
        }
    }
}
﻿using System;

namespace FtpLibDotNet
{
    /// <summary>
    /// File Transfer Protocol Commands (RFC 959)
    /// </summary>
    internal static class FtpCommands
    {
        #region Access Control Commands

        /*  USER NAME (USER)

            The argument field is a Telnet string identifying the user.
            The user identification is that which is required by the
            server for access to its file system.  This command will
            normally be the first command transmitted by the user after
            the control connections are made (some servers may require
            this).  Additional identification information in the form of
            a password and/or an account command may also be required by
            some servers.  Servers may allow a new USER command to be
            entered at any point in order to change the access control
            and/or accounting information.  This has the effect of
            flushing any user, password, and account information already
            supplied and beginning the login sequence again.  All
            transfer parameters are unchanged and any file transfer in
            progress is completed under the old access control
            parameters.
        */

        public static string USER(string username) => String.Format("USER {0}", username);

        /* PASSWORD (PASS)

            The argument field is a Telnet string specifying the user's
            password.  This command must be immediately preceded by the
            user name command, and, for some sites, completes the user's
            identification for access control.  Since password
            information is quite sensitive, it is desirable in general
            to "mask" it or suppress typeout.  It appears that the
            server has no foolproof way to achieve this.  It is
            therefore the responsibility of the user-FTP process to hide
            the sensitive password information.
        */

        public static string PASS(string password) => String.Format("PASS {0}", password);

        /* ACCOUNT (ACCT)

            The argument field is a Telnet string identifying the user's
            account.  The command is not necessarily related to the USER
            command, as some sites may require an account for login and
            others only for specific access, such as storing files.  In
            the latter case the command may arrive at any time.

            There are reply codes to differentiate these cases for the
            automation: when account information is required for login,
            the response to a successful PASSword command is reply code
            332.  On the other hand, if account information is NOT
            required for login, the reply to a successful PASSword
            command is 230; and if the account information is needed for
            a command issued later in the dialogue, the server should
            return a 332 or 532 reply depending on whether it stores
            (pending receipt of the ACCounT command) or discards the
            command, respectively.
        */

        public static string ACCT(string account_information) => String.Format("ACCT {0}", account_information);

        /* CHANGE WORKING DIRECTORY (CWD)

            This command allows the user to work with a different
            directory or dataset for file storage or retrieval without
            altering his login or accounting information.  Transfer
            parameters are similarly unchanged.  The argument is a
            pathname specifying a directory or other system dependent
            file group designator.
        */

        public static string CWD(string pathname) => String.Format("CWD {0}", pathname);

        /* CHANGE TO PARENT DIRECTORY (CDUP)

            This command is a special case of CWD, and is included to
            simplify the implementation of programs for transferring
            directory trees between operating systems having different
            syntaxes for naming the parent directory.  The reply codes
            shall be identical to the reply codes of CWD.  See
            Appendix II for further details.
        */

        public static string CDUP() => "CDUP";

        /* STRUCTURE MOUNT (SMNT)

            This command allows the user to mount a different file
            system data structure without altering his login or
            accounting information.  Transfer parameters are similarly
            unchanged.  The argument is a pathname specifying a
            directory or other system dependent file group designator.
        */

        public static string SMNT(string pathname) => String.Format("SMNT {0}", pathname);

        /* REINITIALIZE (REIN)

            This command terminates a USER, flushing all I/O and account
            information, except to allow any transfer in progress to be
            completed.  All parameters are reset to the default settings
            and the control connection is left open.  This is identical
            to the state in which a user finds himself immediately after
            the control connection is opened.  A USER command may be
            expected to follow.
        */

        public static string REIN() => "REIN";

        /*  LOGOUT (QUIT)

            This command terminates a USER and if file transfer is not
            in progress, the server closes the control connection.  If
            file transfer is in progress, the connection will remain
            open for result response and the server will then close it.
            If the user-process is transferring files for several USERs
            but does not wish to close and then reopen connections for
            each, then the REIN command should be used instead of QUIT.

            An unexpected close on the control connection will cause the
            server to take the effective action of an abort (ABOR) and a
            logout (QUIT).
        */

        public static string QUIT() => "QUIT";

        #endregion

        #region Transfer Parameter Commands

        /* DATA PORT (PORT)

            The argument is a HOST-PORT specification for the data port
            to be used in data connection.  There are defaults for both
            the user and server data ports, and under normal
            circumstances this command and its reply are not needed.  If
            this command is used, the argument is the concatenation of a
            32-bit internet host address and a 16-bit TCP port address.
            This address information is broken into 8-bit fields and the
            value of each field is transmitted as a decimal number (in
            character string representation).  The fields are separated
            by commas.  A port command would be:

               PORT h1,h2,h3,h4,p1,p2

            where h1 is the high order 8 bits of the internet host
            address. 
        */

        public static string PORT(int a1, int a2, int a3, int a4, int p1, int p2) => String.Format("PORT {0},{1},{2},{3},{4},{5}", a1, a2, a3, a4, p1, p2);

        /* PASSIVE (PASV)

            This command requests the server-DTP to "listen" on a data
            port (which is not its default data port) and to wait for a
            connection rather than initiate one upon receipt of a
            transfer command.  The response to this command includes the
            host and port address this server is listening on.
        */

        public static string PASV() => "PASV";

        /* REPRESENTATION TYPE (TYPE)

            The argument specifies the representation type as described
            in the Section on Data Representation and Storage.  Several
            types take a second parameter.  The first parameter is
            denoted by a single Telnet character, as is the second
            Format parameter for ASCII and EBCDIC; the second parameter
            for local byte is a decimal integer to indicate Bytesize.
            The parameters are separated by a <SP> (Space, ASCII code
            32).

            The following codes are assigned for type:

                         \    /
               A - ASCII |    | N - Non-print
                         |-><-| T - Telnet format effectors
               E - EBCDIC|    | C - Carriage Control (ASA)
                         /    \
               I - Image

               L <byte size> - Local byte Byte size

            The default representation type is ASCII Non-print.  If the
            Format parameter is changed, and later just the first
            argument is changed, Format then returns to the Non-print
            default.
        */

        public static string TYPE(string type_code) => String.Format("TYPE {0}", type_code);

        public static string TYPE(string type_code, string form_code) => String.Format("TYPE {0} {1}", type_code, form_code);

        public static string TYPEL(int byte_size) => String.Format("TYPE L {0}", byte_size);

        /* FILE STRUCTURE (STRU)

            The argument is a single Telnet character code specifying
            file structure described in the Section on Data
            Representation and Storage.

            The following codes are assigned for structure:

               F - File (no record structure)
               R - Record structure
               P - Page structure

            The default structure is File.
        */

        public static string STRU(string structure_code) => String.Format("STRU {0}", structure_code);

        /* TRANSFER MODE (MODE)

            The argument is a single Telnet character code specifying
            the data transfer modes described in the Section on
            Transmission Modes.

            The following codes are assigned for transfer modes:

               S - Stream
               B - Block
               C - Compressed

            The default transfer mode is Stream.
        */

        public static string MODE(string mode_code) => String.Format("MODE {0}", mode_code);

        #endregion

        #region FTP Service Commands

        /* RETRIEVE (RETR)

            This command causes the server-DTP to transfer a copy of the
            file, specified in the pathname, to the server- or user-DTP
            at the other end of the data connection.  The status and
            contents of the file at the server site shall be unaffected.
        */

        public static string RETR(string pathname) => String.Format("RETR {0}", pathname);

        /* STORE (STOR)

            This command causes the server-DTP to accept the data
            transferred via the data connection and to store the data as
            a file at the server site.  If the file specified in the
            pathname exists at the server site, then its contents shall
            be replaced by the data being transferred.  A new file is
            created at the server site if the file specified in the
            pathname does not already exist.
        */

        public static string STOR(string pathname) => String.Format("STOR {0}", pathname);

        /* STORE UNIQUE (STOU)

            This command behaves like STOR except that the resultant
            file is to be created in the current directory under a name
            unique to that directory.  The 250 Transfer Started response
            must include the name generated.
        */

        public static string STOU() => "STOU";

        /* APPEND (with create) (APPE)

            This command causes the server-DTP to accept the data
            transferred via the data connection and to store the data in
            a file at the server site.  If the file specified in the
            pathname exists at the server site, then the data shall be
            appended to that file; otherwise the file specified in the
            pathname shall be created at the server site.
        */

        public static string APPE(string pathname) => String.Format("APPE {0}", pathname);

        /*  ALLOCATE (ALLO)

            This command may be required by some servers to reserve
            sufficient storage to accommodate the new file to be
            transferred.  The argument shall be a decimal integer
            representing the number of bytes (using the logical byte
            size) of storage to be reserved for the file.  For files
            sent with record or page structure a maximum record or page
            size (in logical bytes) might also be necessary; this is
            indicated by a decimal integer in a second argument field of
            the command.  This second argument is optional, but when
            present should be separated from the first by the three
            Telnet characters <SP> R <SP>.  This command shall be
            followed by a STORe or APPEnd command.  The ALLO command
            should be treated as a NOOP (no operation) by those servers
            which do not require that the maximum size of the file be
            declared beforehand, and those servers interested in only
            the maximum record or page size should accept a dummy value
            in the first argument and ignore it.
        */

        public static string ALLO(int size) => String.Format("ALLO {0}", size);

        public static string ALLO(int size, int max_size) => String.Format("ALLO {0} R {1}", size, max_size);

        /* RESTART (REST)

            The argument field represents the server marker at which
            file transfer is to be restarted.  This command does not
            cause file transfer but skips over the file to the specified
            data checkpoint.  This command shall be immediately followed
            by the appropriate FTP service command which shall cause
            file transfer to resume.
        */

        public static string REST(string marker) => String.Format("REST {0}", marker);

        /* RENAME FROM (RNFR)

            This command specifies the old pathname of the file which is
            to be renamed.  This command must be immediately followed by
            a "rename to" command specifying the new file pathname.
        */

        public static string RNFR(string pathname) => String.Format("RNFR {0}", pathname);

        /* RENAME TO (RNTO)

            This command specifies the new pathname of the file
            specified in the immediately preceding "rename from"
            command.  Together the two commands cause a file to be
            renamed.
        */

        public static string RNTO(string pathname) => String.Format("RNTO {0}", pathname);

        /* ABORT (ABOR)

            This command tells the server to abort the previous FTP
            service command and any associated transfer of data.  The
            abort command may require "special action", as discussed in
            the Section on FTP Commands, to force recognition by the
            server.  No action is to be taken if the previous command
            has been completed (including data transfer).  The control
            connection is not to be closed by the server, but the data
            connection must be closed.

            There are two cases for the server upon receipt of this
            command: (1) the FTP service command was already completed,
            or (2) the FTP service command is still in progress.
            In the first case, the server closes the data connection
            (if it is open) and responds with a 226 reply, indicating
            that the abort command was successfully processed.

            In the second case, the server aborts the FTP service in
            progress and closes the data connection, returning a 426
            reply to indicate that the service request terminated
            abnormally.  The server then sends a 226 reply,
            indicating that the abort command was successfully
            processed.
        */

        public static string ABOR() => "ABOR";

        /* DELETE (DELE)

            This command causes the file specified in the pathname to be
            deleted at the server site.  If an extra level of protection
            is desired (such as the query, "Do you really wish to
            delete?"), it should be provided by the user-FTP process.
        */

        public static string DELE(string pathname) => String.Format("DELE {0}", pathname);

        /* REMOVE DIRECTORY (RMD)

            This command causes the directory specified in the pathname
            to be removed as a directory (if the pathname is absolute)
            or as a subdirectory of the current working directory (if
            the pathname is relative).  See Appendix II.
        */

        public static string RMD(string pathname) => String.Format("RMD {0}", pathname);

        /* MAKE DIRECTORY (MKD)

            This command causes the directory specified in the pathname
            to be created as a directory (if the pathname is absolute)
            or as a subdirectory of the current working directory (if
            the pathname is relative).  See Appendix II.
        */

        public static string MKD(string pathname) => String.Format("MKD {0}", pathname);

        /* PRINT WORKING DIRECTORY (PWD)

            This command causes the name of the current working
            directory to be returned in the reply.  See Appendix II.
        */

        public static string PWD() => "PWD";

        /* LIST (LIST)

            This command causes a list to be sent from the server to the
            passive DTP.  If the pathname specifies a directory or other
            group of files, the server should transfer a list of files
            in the specified directory.  If the pathname specifies a
            file then the server should send current information on the
            file.  A null argument implies the user's current working or
            default directory.  The data transfer is over the data
            connection in type ASCII or type EBCDIC.  (The user must
            ensure that the TYPE is appropriately ASCII or EBCDIC).
            Since the information on a file may vary widely from system
            to system, this information may be hard to use automatically
            in a program, but may be quite useful to a human user.
        */

        public static string LIST() => "LIST";

        public static string LIST(string pathname) => String.Format("LIST {0}", pathname);

        /* NAME LIST (NLST)

            This command causes a directory listing to be sent from
            server to user site.  The pathname should specify a
            directory or other system-specific file group descriptor; a
            null argument implies the current directory.  The server
            will return a stream of names of files and no other
            information.  The data will be transferred in ASCII or
            EBCDIC type over the data connection as valid pathname
            strings separated by <CRLF> or <NL>.  (Again the user must
            ensure that the TYPE is correct.)  This command is intended
            to return information that can be used by a program to
            further process the files automatically.  For example, in
            the implementation of a "multiple get" function.
        */

        public static string NLST() => "NLST";

        public static string NLST(string pathname) => String.Format("NLST {0}", pathname);

        /* SITE PARAMETERS (SITE)

            This command is used by the server to provide services
            specific to his system that are essential to file transfer
            but not sufficiently universal to be included as commands in
            the protocol.  The nature of these services and the
            specification of their syntax can be stated in a reply to
            the HELP SITE command.
        */

        public static string SITE(string site_specific_cmd) => String.Format("SITE {0}", site_specific_cmd);

        /* SYSTEM (SYST)

            This command is used to find out the type of operating
            system at the server.  The reply shall have as its first
            word one of the system names listed in the current version
            of the Assigned Numbers document [4].
        */

        public static string SYST() => "SYST";

        /* STATUS (STAT)

            This command shall cause a status response to be sent over
            the control connection in the form of a reply.  The command
            may be sent during a file transfer (along with the Telnet IP
            and Synch signals--see the Section on FTP Commands) in which
            case the server will respond with the status of the
            operation in progress, or it may be sent between file
            transfers.  In the latter case, the command may have an
            argument field.  If the argument is a pathname, the command
            is analogous to the "list" command except that data shall be
            transferred over the control connection.  If a partial
            pathname is given, the server may respond with a list of
            file names or attributes associated with that specification.
            If no argument is given, the server should return general
            status information about the server FTP process.  This
            should include current values of all transfer parameters and
            the status of connections.
        */

        public static string STAT() => "STAT";

        public static string STAT(string pathname) => String.Format("STAT {0}", pathname);

        /* HELP (HELP)

            This command shall cause the server to send helpful
            information regarding its implementation status over the
            control connection to the user.  The command may take an
            argument (e.g., any command name) and return more specific
            information as a response.  The reply is type 211 or 214.
            It is suggested that HELP be allowed before entering a USER
            command. The server may use this reply to specify
            site-dependent parameters, e.g., in response to HELP SITE.
        */

        public static string HELP() => "HELP";

        public static string HELP(string command) => String.Format("HELP {0}", command);

        /* NOOP (NOOP)

            This command does not affect any parameters or previously
            entered commands. It specifies no action other than that the
            server send an OK reply.
        */

        public static string NOOP() => "NOOP";

        #endregion
    }
}

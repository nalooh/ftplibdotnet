namespace FtpLibDotNet
{
    public enum FtpReplyCode : int
    {
        /// <summary>
        /// 200 Command okay.
        /// </summary>
        OK = 200,
        /// <summary>
        /// 500 Syntax error, command unrecognized.
        /// This may include errors such as command line too long.
        /// </summary>
        CommandUnrecognized = 500,
        /// <summary>
        /// 501 Syntax error in parameters or arguments.
        /// </summary>
        SyntaxErrorInParameters = 501,
        /// <summary>
        /// 202 Command not implemented, superfluous at this site.
        /// </summary>
        CommandNotImplementedSuperfluousAtThisSite = 202,
        /// <summary>
        /// 502 Command not implemented.
        /// </summary>
        CommandNotImplemented = 502,
        /// <summary>
        /// 503 Bad sequence of commands.
        /// </summary>
        BadSequenceOfCommands = 503,
        /// <summary>
        /// 504 Command not implemented for that parameter.
        /// </summary>
        CommandNotImplementedForThatParameter = 504,
        /// <summary>
        /// 110 Restart marker reply.
        /// In this case, the text is exact and not left to the particular implementation; it must read: MARK yyyy = mmmm Where yyyy is User-process data stream marker, and mmmm server's equivalent marker (note the spaces between markers and "=").
        /// </summary>
        RestartMarkerReply = 110,
        /// <summary>
        /// 211 System status, or system help reply.
        /// </summary>
        SystemStatus = 221,
        /// <summary>
        /// 212 Directory status.
        /// </summary>
        DirectoryStatus = 212,
        /// <summary>
        /// 213 File status.
        /// </summary>
        FileStatus = 213,
        /// <summary>
        /// 214 Help message.
        /// On how to use the server or the meaning of a particular non-standard command.  This reply is useful only to the human user.
        /// </summary>
        HelpMessage = 214,
        /// <summary>
        /// 215 NAME system type.
        /// Where NAME is an official system name from the list in the Assigned Numbers document.
        /// </summary>
        SystemType = 215,
        /// <summary>
        /// 120 Service ready in nnn minutes.
        /// </summary>
        ServiceToBeReadyIn = 120,
        /// <summary>
        /// 220 Service ready for new user.
        /// </summary>
        ServiceReadyForNewUser = 220,
        /// <summary>
        /// 221 Service closing control connection. Logged out if appropriate.
        /// </summary>
        ServiceClosingConnection = 221,
        /// <summary>
        /// 421 Service not available, closing control connection.
        /// This may be a reply to any command if the service knows it must shut down.
        /// </summary>
        ServiceNotAvailable = 421,
        /// <summary>
        /// 125 Data connection already open; transfer starting.
        /// </summary>
        DataConnectionAlreadyOpenTransferStarting = 125,
        /// <summary>
        /// 225 Data connection open; no transfer in progress.
        /// </summary>
        DataConnectionAlreadyOpen = 225,
        /// <summary>
        /// 425 Can't open data connection.
        /// </summary>
        CantOpenDataConnection = 425,
        /// <summary>
        /// 226 Closing data connection. Requested file action successful (for example, file transfer or file abort).
        /// </summary>
        ClosingDataConnection = 226,
        /// <summary>
        /// 426 Connection closed; transfer aborted.
        /// </summary>
        TransferAborted = 426,
        /// <summary>
        /// 227 Entering Passive Mode (h1,h2,h3,h4,p1,p2).
        /// </summary>
        EnteringPassiveMode = 227,
        /// <summary>
        /// 230 User logged in, proceed.
        /// </summary>
        UserLoggedIn = 230,
        /// <summary>
        /// 530 Not logged in.
        /// </summary>
        NotLoggedIn = 530,
        /// <summary>
        /// 331 User name okay, need password.
        /// </summary>
        NeedPassword = 331,
        /// <summary>
        /// 332 Need account for login.
        /// </summary>
        NeedAccountForLogin = 332,
        /// <summary>
        /// 532 Need account for storing files.
        /// </summary>
        NeedAccountForStoringFiles = 532,
        /// <summary>
        /// 150 File status okay; about to open data connection.
        /// </summary>
        FileStatusOk = 150,
        /// <summary>
        /// 250 Requested file action okay, completed.
        /// </summary>
        RequestedFileActionCompleted = 250,
        /// <summary>
        /// 257 "PATHNAME" created.
        /// </summary>
        PathCreated = 257,
        /// <summary>
        /// 350 Requested file action pending further information.
        /// </summary>
        RequestedFileActionPendingFurtherInformation = 350,
        /// <summary>
        /// 450 Requested file action not taken. File unavailable (e.g., file busy).
        /// </summary>
        FileUnavailableBusy = 450,
        /// <summary>
        /// 550 Requested action not taken. File unavailable (e.g., file not found, no access).
        /// </summary>
        FileUnavailableNotfound = 550,

        /*
         451 Requested action aborted. Local error in processing.
         551 Requested action aborted. Page type unknown.
         452 Requested action not taken.
             Insufficient storage space in system.
         552 Requested file action aborted.
             Exceeded storage allocation (for current directory or
             dataset).
         553 Requested action not taken.
             File name not allowed.
     */
    }
}

/*
200  Command okay                                             11i1
500  Syntax error, command unrecognized
        [This may include errors such as command line too
        long.]                                                11i2
501  Syntax error in parameters or arguments                  11i3
202  Command not imlemented, superfluous at this site.        11i4
502  Command not implemented                                  11i5
503  Bad sequence of commands                                 11i6
504  Command not implemented for that parameter               11i7
                                                                11j
110  Restart marker reply.
        In this case the text is exact and not left to the
        particular implementation; it must read:
                    MARK yyyy = mmmm
        where yyyy is User-process data stream marker, and
        mmmm is Server's equivalent marker.  (note the
        spaces between the markers and "=".)                  11j1
211  System status, or system help reply                      11j2
212  Directory status                                         11j3
213  File status                                              11j4
214  Help message (on how to use the server or the meaning
        of a particular non-standard command.  This reply
        is useful only to the human user.)                    11j5
                                                                11k
120  Service ready in nnn minutes                             11k1
220  Service ready for new user                               11k2
221  Service closing TELNET connection (logged off if
        appropriate)                                          11k3
421  Service not available, closing TELNET connection.
        [This may be a reply to any command if the service
        knows it must shut down.]                             11k4
125  Data connection already open; transfer starting          11k5
225  Data connection open; no transfer in progress            11k6
425  Can't open data connection                               11k7
226  Closing data connection; requested file action
        successful (for example, file transfer or file
        abort.)                                               11k8
426  Connection trouble, closed; transfer aborted.            11k9
227  Entering [passive, active] mode                         11k10
                                                                11l
230  User logged on, proceed                                  11l1
530  Not logged in                                            11l2
331  User name okay, need password                            11l3
332  Need account for login                                   11l4
532  Need account for storing files                           11l5
                                                                11m
150  File status okay; about to open data connection.         11m1
250  Requested file action okay, completed.                   11m2
350  Requested file action pending further information        11m3
450  Requested file action not taken: file unavailable
        (e.g. file not found, no access)                      11m4
550  Requested action not taken:  file unavailable (e.g.
        file busy)                                            11m5
451  Requested action aborted: local error in processing      11m6
452  Requested action not taken:  insufficient storage
        space in system                                       11m7
552  Requested file action aborted:  exceeded storage
        allocation (for current directory or dataset)         11m8
553  Requested action not taken: file name not allowed        11m9
354  Start mail input; end with <CR><LF>.<CR><LF>            11m10
*/

/*
Connection Establishment
    120
        220
    220
    421
Login
    USER
        230
        530
        500, 501, 421
        331, 332
    PASS
        230
        202
        530
        500, 501, 503, 421
        332
    ACCT
        230
        202
        530
        500, 501, 503, 421
    CWD
        250
        500, 501, 502, 421, 530, 550
    CDUP
        200
        500, 501, 502, 421, 530, 550
    SMNT
        202, 250
        500, 501, 502, 421, 530, 550
Logout
    REIN
        120
            220
        220
        421
        500, 502
    QUIT
        221
        500
Transfer parameters
    PORT
        200
        500, 501, 421, 530
    PASV
        227
        500, 501, 502, 421, 530
    MODE
        200
        500, 501, 504, 421, 530
    TYPE
        200
        500, 501, 504, 421, 530
    STRU
        200
        500, 501, 504, 421, 530
File action commands
    ALLO
        200
        202
        500, 501, 504, 421, 530
    REST
        500, 501, 502, 421, 530
        350
    STOR
        125, 150
            (110)
            226, 250
            425, 426, 451, 551, 552
        532, 450, 452, 553
        500, 501, 421, 530
    STOU
        125, 150
            (110)
            226, 250
            425, 426, 451, 551, 552
        532, 450, 452, 553
        500, 501, 421, 530
    RETR
        125, 150
            (110)
            226, 250
            425, 426, 451
        450, 550
        500, 501, 421, 530
    LIST
        125, 150
            226, 250
            425, 426, 451
        450
        500, 501, 502, 421, 530
    NLST
        125, 150
            226, 250
            425, 426, 451
        450
        500, 501, 502, 421, 530
    APPE
        125, 150
            (110)
            226, 250
            425, 426, 451, 551, 552
        532, 450, 550, 452, 553
        500, 501, 502, 421, 530
    RNFR
        450, 550
        500, 501, 502, 421, 530
        350
    RNTO
        250
        532, 553
        500, 501, 502, 503, 421, 530
    DELE
        250
        450, 550
        500, 501, 502, 421, 530
    RMD
        250
        500, 501, 502, 421, 530, 550
    MKD
        257
        500, 501, 502, 421, 530, 550
    PWD
        257
        500, 501, 502, 421, 550
    ABOR
        225, 226
        500, 501, 502, 421
Informational commands
    SYST
        215
        500, 501, 502, 421
    STAT
        211, 212, 213
        450
        500, 501, 502, 421, 530
    HELP
        211, 214
        500, 501, 502, 421
Miscellaneous commands
    SITE
        200
        202
        500, 501, 530
    NOOP
        200
        500 421
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightningOS.Utilities
{
    class FirstRunFileContents
    {
        
		public static string mkdirCommandBin = @"
.cref
.list

ifndef  Kanji
Kanji   equ 0
endif

        i_need  AUXSTACK,BYTE
        i_need  NoSetDir,BYTE
        i_need  CURBUF, DWORD
        i_need  DIRSTART,WORD
        i_need  THISDPB,DWORD
        i_need  NAME1,BYTE
        i_need  LASTENT,WORD
        i_need  ATTRIB,BYTE
        i_need  THISFCB,DWORD
        i_need  AUXSTACK,BYTE
        i_need  CREATING,BYTE
        i_need  DRIVESPEC,BYTE
        i_need  ROOTSTART,BYTE
        i_need  SWITCH_CHARACTER,BYTE

        extrn   sys_ret_ok:near,sys_ret_err:near


; XENIX CALLS
BREAK <$MkDir - Make a directory entry>
MKNERRJ: JMP    MKNERR
NODEEXISTSJ: JMP NODEEXISTS
        procedure   $MKDIR,NEAR
ASSUME  DS:NOTHING,ES:NOTHING

; Inputs:
;       DS:DX Points to asciz name
; Function:
;       Make a new directory
; Returns:
;       STD XENIX Return
;       AX = mkdir_path_not_found if path bad
;       AX = mkdir_access_denied  If
;               Directory cannot be created
;               Node already exists
;               Device name given
;               Disk or directory(root) full
        invoke  validate_path
        JC      MKNERRJ
        MOV     SI,DX
        MOV     WORD PTR [THISFCB+2],SS
        MOV     WORD PTR [THISFCB],OFFSET DOSGROUP:AUXSTACK-40  ; Scratch space
        MOV     AL,attr_directory
        MOV     WORD PTR [CREATING],0E500h
        invoke  MAKENODE
ASSUME  DS:DOSGROUP
        MOV     AL,mkdir_path_not_found
        JC      MKNERRJ
        JNZ     NODEEXISTSJ
        LDS     DI,[CURBUF]
ASSUME  DS:NOTHING
        SUB     SI,DI
        PUSH    SI              ; Pointer to fcb_FIRCLUS
        PUSH    [DI.BUFSECNO]   ; Sector of new node
        PUSH    SS
        POP     DS
ASSUME  DS:DOSGROUP
        PUSH    [DIRSTART]      ; Parent for .. entry
        XOR     AX,AX
        MOV     [DIRSTART],AX   ; Null directory
        invoke  NEWDIR
        JC      NODEEXISTSPOPDEL    ; No room
        invoke  GETENT          ; First entry
        LES     DI,[CURBUF]
        MOV     ES:[DI.BUFDIRTY],1
        ADD     DI,BUFINSIZ     ; Point at buffer
        MOV     AX,202EH        ; "". ""
        STOSW
        MOV     DX,[DIRSTART]   ; Point at itself
        invoke  SETDOTENT
        MOV     AX,2E2EH        ; ""..""
        STOSW
        POP     DX              ; Parent
        invoke  SETDOTENT
        LES     BP,[THISDPB]
        POP     DX              ; Entry sector
        XOR     AL,AL           ; Pre read
        invoke  GETBUFFR
        MOV     DX,[DIRSTART]
        LDS     DI,[CURBUF]
ASSUME  DS:NOTHING
ZAPENT:
        POP     SI              ; fcb_Firclus pointer
        ADD     SI,DI
        MOV     [SI],DX
        XOR     DX,DX
        MOV     [SI+2],DX
        MOV     [SI+4],DX
DIRUP:
        MOV     [DI.BUFDIRTY],1
        PUSH    SS
        POP     DS
ASSUME  DS:DOSGROUP
        MOV     AL,ES:[BP.dpb_drive]
        invoke  FLUSHBUF
SYS_RET_OKJ:
        JMP     SYS_RET_OK

NODEEXISTSPOPDEL:
        POP     DX              ; Parent
        POP     DX              ; Entry sector
        LES     BP,[THISDPB]
        XOR     AL,AL           ; Pre read
        invoke  GETBUFFR
        LDS     DI,[CURBUF]
ASSUME  DS:NOTHING
        POP     SI              ; dir_first pointer
        ADD     SI,DI
        SUB     SI,dir_first    ; Point back to start of dir entry
        MOV     BYTE PTR [SI],0E5H    ; Free the entry
        CALL    DIRUP
NODEEXISTS:
        MOV     AL,mkdir_access_denied
MKNERR:
        JMP     SYS_RET_ERR
$MKDIR  ENDP

BREAK <$ChDir -- Change current directory on a drive>
        procedure   $CHDIR,NEAR
ASSUME  DS:NOTHING,ES:NOTHING

; Inputs:
;       DS:DX Points to asciz name
; Function:
;       Change current directory
; Returns:
;       STD XENIX Return
;       AX = chdir_path_not_found if error

        invoke  validate_path
        JC      PathTooLong

        PUSH    DS
        PUSH    DX
        MOV     SI,DX
        invoke  GETPATH
        JC      PATHNOGOOD
        JNZ     PATHNOGOOD
ASSUME  DS:DOSGROUP
        MOV     AX,[DIRSTART]
        MOV     BX,AX
        XCHG    BX,ES:[BP.dpb_current_dir]
        OR      AX,AX
        POP     SI
        POP     DS
ASSUME  DS:NOTHING
        JZ      SYS_RET_OKJ
        MOV     DI,BP
        ADD     DI,dpb_dir_text
        MOV     DX,DI
        CMP     [DRIVESPEC],0
        JZ      NODRIVESPEC
        INC     SI
        INC     SI
NODRIVESPEC:
        MOV     CX,SI
        CMP     [ROOTSTART],0
        JZ      NOTROOTPATH
        INC     SI
        INC     CX
        JMP     SHORT COPYTHESTRINGBXZ
NOTROOTPATH:
        OR      BX,BX           ; Previous path root?
        JZ      COPYTHESTRING   ; Yes
        XOR     BX,BX
ENDLOOP:
        CMP     BYTE PTR ES:[DI],0
        JZ      PATHEND
        INC     DI
        INC     BX
        JMP     SHORT ENDLOOP
PATHEND:
        MOV     AL,'/'
        CMP     AL,[switch_character]
        JNZ     SLASHOK
        MOV     AL,'\'                  ; Use the alternate character
SLASHOK:
        STOSB
        INC     BX
        JMP     SHORT CHECK_LEN

PATHNOGOOD:
        POP     AX
        POP     AX
PATHTOOLONG:
        error   error_path_not_found

ASSUME  DS:NOTHING

INCBXCHK:
        INC     BX
BXCHK:
        CMP     BX,DIRSTRLEN
        return

COPYTHESTRINGBXZ:
        XOR     BX,BX
COPYTHESTRING:
        LODSB
        OR      AL,AL

        JNZ     FOOB
        JMP     CPSTDONE
FOOB:
        CMP     AL,'.'
        JZ      SEEDOT
        CALL    COPYELEM
CHECK_LEN:
        CMP     BX,DIRSTRLEN
        JB      COPYTHESTRING
        MOV     AL,ES:[DI-1]
        invoke  PATHCHRCMP
        JNZ     OK_DI
        DEC     DI
OK_DI:
        XOR     AL,AL
        STOSB                   ; Correctly terminate the path
        MOV     ES:[BP.dpb_current_dir],-1      ; Force re-validation
        JMP     SHORT PATHTOOLONG

SEEDOT:
        LODSB
        OR      AL,AL           ; Check for null
        JZ      CPSTDONEDEC
        CMP     AL,'.'
        JNZ     COPYTHESTRING   ; eat ./
        CALL    DELELMES        ; have   ..
        LODSB                   ; eat the /
        OR      AL,AL           ; Check for null
        JZ      CPSTDONEDEC
        JMP     SHORT COPYTHESTRING

; Copy one element from DS:SI to ES:DI include trailing / not trailing null
; LODSB has already been done
COPYELEM:
        PUSH    DI                      ; Save in case too long
        PUSH    CX
        MOV     CX,800h                 ; length of filename
        MOV     AH,'.'                  ; char to stop on
        CALL    CopyPiece               ; go for it!
        CALL    BXCHK                   ; did we go over?
        JAE     POPCXDI                 ; yep, go home
        CMP     AH,AL                   ; did we stop on .?
        JZ      CopyExt                 ; yes, go copy ext
        OR      AL,AL                   ; did we end on nul?
        JZ      DECSIRet                ; yes, bye
CopyPathEnd:
        STOSB                           ; save the path char
        CALL    INCBXCHK                ; was there room for it?
        JAE     POPCXDI                 ; Nope
        INC     SI                      ; guard against following dec
DECSIRET:
        DEC     SI                      ; point back at null
        POP     CX
        POP     AX                      ; toss away saved DI
        return
POPCXDI:
        POP     CX                      ; restore
        POP     DI                      ; point back...
        return
CopyExt:
        STOSB                           ; save the dot
        CALL    INCBXCHK                ; room?
        JAE     POPCXDI                 ; nope.
        LODSB                           ; get next char
        XOR     AH,AH                   ; NUL here
        MOV     CX,300h                 ; at most 3 chars
        CALL    CopyPiece               ; go copy it
        CALL    BXCHK                   ; did we go over
        JAE     POPCXDI                 ; yep
        OR      AL,AL                   ; sucessful end?
        JZ      DECSIRET                ; yes
        JMP     CopyPathEnd             ; go stash path char

DELELMES:
; Delete one path element from ES:DI
        DEC     DI                      ; the '/'
        DEC     BX

        IF      KANJI
        PUSH    AX
        PUSH    CX
        PUSH    DI
        PUSH    DX
        MOV     CX,DI
        MOV     DI,DX
DELLOOP:
        CMP     DI,CX
        JZ      GOTDELE
        MOV     AL,ES:[DI]
        INC     DI
        invoke  TESTKANJ
        JZ      NOTKANJ11
        INC     DI
        JMP     DELLOOP

NOTKANJ11:
        invoke  PATHCHRCMP
        JNZ     DELLOOP
        MOV     DX,DI                   ; Point to char after '/'
        JMP     DELLOOP

GOTDELE:
        MOV     DI,DX
        POP     DX
        POP     AX                      ; Initial DI
        SUB     AX,DI                   ; Distance moved
        SUB     BX,AX                   ; Set correct BX
        POP     CX
        POP     AX
        return
        ELSE
DELLOOP:
        CMP     DI,DX
        retz
        PUSH    AX
        MOV     AL,ES:[DI-1]
        invoke  PATHCHRCMP
        POP     AX
        retz
        DEC     DI
        DEC     BX
        JMP     SHORT DELLOOP
        ENDIF

CPSTDONEDEC:
        DEC     DI                      ; Back up over trailing /
CPSTDONE:
        STOSB                           ; The NUL
        JMP     SYS_RET_OK

; copy a piece CH chars max until the char in AH (or path or NUL)
CopyPiece:
        STOSB                           ; store the character
        INC     CL                      ; moved a byte
        CALL    INCBXCHK                ; room enough?
        JAE     CopyPieceRet            ; no, pop CX and DI
        OR      AL,AL                   ; end of string?
        JZ      CopyPieceRet            ; yes, dec si and return

        IF KANJI
        CALL    TestKanj                ; was it kanji?
        JZ      NotKanj                 ; nope
        MOVSB                           ; move the next byte
        CALL    INCBXCHK                ; room for it?
        JAE     CopyPieceRet            ; nope
        INC     CL                      ; moved a byte
NotKanj:
        ENDIF

        CMP     CL,CH                   ; move too many?
        JBE     CopyPieceNext           ; nope

        IF KANJI
        CALL    TestKanj                ; was the last byte kanji
        JZ      NotKanj2                ; no only single byte backup
        DEC     DI                      ; back up a char
        DEC     BX
NotKanj2:
        ENDIF

        DEC     DI                      ; back up a char
        DEC     BX
CopyPieceNext:
        LODSB                           ; get next character
        invoke  PathChrCmp              ; end of road?
        JZ      CopyPieceRet            ; yep, return and don't dec SI
        CMP     AL,AH                   ; end of filename?
        JNZ     CopyPiece               ; go do name
CopyPieceRet:
        return                          ; bye!

$CHDIR  ENDP

BREAK <$RmDir -- Remove a directory>
NOPATHJ: JMP    NOPATH

        procedure   $RMDIR,NEAR         ; System call 47
ASSUME  DS:NOTHING,ES:NOTHING

; Inputs:
;       DS:DX Points to asciz name
; Function:
;       Delete directory if empty
; Returns:
;       STD XENIX Return
;       AX = rmdir_path_not_found If path bad
;       AX = rmdir_access_denied If
;               Directory not empty
;               Path not directory
;               Root directory specified
;               Directory malformed (. and .. not first two entries)
;       AX = rmdir_current_directory

        invoke  Validate_path
        JC      NoPathJ
        MOV     SI,DX
        invoke  GETPATH
        JC      NOPATHJ
ASSUME  DS:DOSGROUP
        JNZ     NOTDIRPATH
        MOV     DI,[DIRSTART]
        OR      DI,DI
        JZ      NOTDIRPATH
        MOV     CX,ES:[BP.dpb_current_dir]
        CMP     CX,-1
        JNZ     rmdir_current_dir_check
        invoke  GetCurrDir
        invoke  Get_user_stack
        MOV     DX,[SI.user_DX]
        MOV     DS,[SI.user_DS]
        JMP     $RMDIR

NOTDIRPATHPOP:
        POP     AX
        POP     AX
NOTDIRPATH:
        error   error_access_denied

rmdir_current_dir_check:
        CMP     DI,CX
        JNZ     rmdir_get_buf
        error   error_current_directory

rmdir_get_buf:
        LDS     DI,[CURBUF]
ASSUME  DS:NOTHING
        SUB     BX,DI
        PUSH    BX                      ; Save entry pointer
        PUSH    [DI.BUFSECNO]           ; Save sector number
        PUSH    SS
        POP     DS
ASSUME  DS:DOSGROUP
        PUSH    SS
        POP     ES
        MOV     DI,OFFSET DOSGROUP:NAME1
        MOV     AL,'?'
        MOV     CX,11
        REP     STOSB
        XOR     AL,AL
        STOSB
        invoke  STARTSRCH
        invoke  GETENTRY
        MOV     DS,WORD PTR [CURBUF+2]
ASSUME  DS:NOTHING
        MOV     SI,BX
        LODSW
        CMP     AX,(' ' SHL 8) OR '.'
        JNZ     NOTDIRPATHPOP
        ADD     SI,32-2
        LODSW
        CMP     AX,('.' SHL 8) OR '.'
        JNZ     NOTDIRPATHPOP
        PUSH    SS
        POP     DS
ASSUME  DS:DOSGROUP
        MOV     [LASTENT],2             ; Skip . and ..
        invoke  GETENTRY
        MOV     [ATTRIB],attr_directory+attr_hidden+attr_system
        invoke  SRCH
        JNC     NOTDIRPATHPOP
        LES     BP,[THISDPB]
        MOV     BX,[DIRSTART]
        invoke  RELEASE
        POP     DX
        XOR     AL,AL
        invoke  GETBUFFR
        LDS     DI,[CURBUF]
ASSUME  DS:NOTHING
        POP     BX
        ADD     BX,DI
        MOV     BYTE PTR [BX],0E5H      ; Free the entry
        JMP     DIRUP

NOPATH:
        error   error_path_not_found

$RMDIR  ENDP

        do_ext

CODE    ENDS
        END
";

        public static string copyCommandSource = @"
DATARES SEGMENT PUBLIC
        EXTRN   VERVAL:WORD
DATARES ENDS

TRANDATA        SEGMENT PUBLIC
        EXTRN   BADARGS:BYTE,BADCD:BYTE,BADSWT:BYTE,COPIED_PRE:BYTE
        EXTRN   COPIED_POST:BYTE
        EXTRN   INBDEV:BYTE,OVERWR:BYTE,FULDIR:BYTE,LOSTERR:BYTE
        EXTRN   NOSPACE:BYTE,DEVWMES:BYTE,NOTFND:BYTE
TRANDATA        ENDS

TRANSPACE       SEGMENT PUBLIC
        EXTRN   MELCOPY:BYTE,SRCPT:WORD,MELSTART:WORD,SCANBUF:BYTE
        EXTRN   DESTFCB2:BYTE,SDIRBUF:BYTE,SRCTAIL:WORD,CFLAG:BYTE
        EXTRN   NXTADD:WORD,DESTCLOSED:BYTE,ALLSWITCH:WORD,ARGC:BYTE
        EXTRN   PLUS:BYTE,BINARY:BYTE,ASCII:BYTE,FILECNT:WORD
        EXTRN   WRITTEN:BYTE,CONCAT:BYTE,DESTBUF:BYTE,SRCBUF:BYTE
        EXTRN   SDIRBUF:BYTE,DIRBUF:BYTE,DESTFCB:BYTE,FRSTSRCH:BYTE
        EXTRN   FIRSTDEST:BYTE,DESTISDIR:BYTE,DESTSWITCH:WORD,STARTEL:WORD
        EXTRN   DESTTAIL:WORD,DESTSIZ:BYTE,DESTINFO:BYTE,INEXACT:BYTE
        EXTRN   CURDRV:BYTE,DESTVARS:BYTE,RESSEG:WORD,SRCSIZ:BYTE
        EXTRN   SRCINFO:BYTE,SRCVARS:BYTE,USERDIR1:BYTE,NOWRITE:BYTE
        EXTRN   RDEOF:BYTE,SRCHAND:WORD,CPDATE:WORD,CPTIME:WORD
        EXTRN   SRCISDEV:BYTE,BYTCNT:WORD,TPA:WORD,TERMREAD:BYTE
        EXTRN   DESTHAND:WORD,DESTISDEV:BYTE,DIRCHAR:BYTE
TRANSPACE       ENDS


; **************************************************
; COPY CODE
;

TRANCODE        SEGMENT PUBLIC BYTE

        EXTRN   RESTUDIR:NEAR,CERROR:NEAR,SWITCH:NEAR,DISP32BITS:NEAR
        EXTRN   PRINT:NEAR,TCOMMAND:NEAR,ZPRINT:NEAR,ONESPC:NEAR
        EXTRN   RESTUDIR1:NEAR,FCB_TO_ASCZ:NEAR,CRLF2:NEAR,SAVUDIR1:NEAR
        EXTRN   SETREST1:NEAR,BADCDERR:NEAR,STRCOMP:NEAR,DELIM:NEAR
        EXTRN   UPCONV:NEAR,PATHCHRCMP:NEAR,SCANOFF:NEAR

        EXTRN   CPARSE:NEAR

        EXTRN   SEARCH:NEAR,SEARCHNEXT:NEAR,DOCOPY:NEAR,CLOSEDEST:NEAR
        EXTRN   FLSHFIL:NEAR,SETASC:NEAR,BUILDNAME:NEAR,COPERR:NEAR

        PUBLIC  COPY,BUILDPATH,COMPNAME,ENDCOPY


ASSUME  CS:TRANGROUP,DS:TRANGROUP,ES:TRANGROUP,SS:NOTHING

DOMELCOPY:
        cmp     [MELCOPY],0FFH
        jz      CONTMEL
        mov     SI,[SRCPT]
        mov     [MELSTART],si
        mov     [MELCOPY],0FFH
CONTMEL:
        xor     BP,BP
        mov     si,[SRCPT]
        mov     bl,'+'
SCANSRC2:
        mov     di,OFFSET TRANGROUP:SCANBUF
        call    CPARSE
        test    bh,80H
        jz      NEXTMEL                 ; Go back to start
        test    bh,1                    ; Switch ?
        jnz     SCANSRC2                ; Yes
        call    SOURCEPROC
        call    RESTUDIR1
        mov     di,OFFSET TRANGROUP:DESTFCB2
        mov     ax,PARSE_FILE_DESCRIPTOR SHL 8
        INT     int_command
        mov     bx,OFFSET TRANGROUP:SDIRBUF + 1
        mov     si,OFFSET TRANGROUP:DESTFCB2 + 1
        mov     di,[SRCTAIL]
        call    BUILDNAME
        jmp     MELDO


NEXTMEL:
        call    CLOSEDEST
        xor     ax,ax
        mov     [CFLAG],al
        mov     [NXTADD],ax
        mov     [DESTCLOSED],al
        mov     si,[MELSTART]
        mov     [SRCPT],si
        call    SEARCHNEXT
        jz      SETNMELJ
        jmp     ENDCOPY2
SETNMELJ:
        jmp     SETNMEL

COPY:
; First order of buisness is to find out about the destination
ASSUME  DS:TRANGROUP,ES:TRANGROUP
        xor     ax,ax
        mov     [ALLSWITCH],AX          ; no switches
        mov     [ARGC],al               ; no arguments
        mov     [PLUS],al               ; no concatination
        mov     [BINARY],al             ; Binary not specifically specified
        mov     [ASCII],al              ; ASCII not specifically specified
        mov     [FILECNT],ax            ; No files yet
        mov     [WRITTEN],al            ; Nothing written yet
        mov     [CONCAT],al             ; No concatination
        mov     [MELCOPY],al            ; Not a Mel Hallerman copy
        mov     word ptr [SCANBUF],ax   ; Init buffer
        mov     word ptr [DESTBUF],ax   ; Init buffer
        mov     word ptr [SRCBUF],ax    ; Init buffer
        mov     word ptr [SDIRBUF],ax   ; Init buffer
        mov     word ptr [DIRBUF],ax    ; Init buffer
        mov     word ptr [DESTFCB],ax   ; Init buffer
        dec     ax
        mov     [FRSTSRCH],al           ; First search call
        mov     [FIRSTDEST],al          ; First time
        mov     [DESTISDIR],al          ; Don't know about dest
        mov     si,81H
        mov     bl,'+'                  ; include '+' as a delimiter
DESTSCAN:
        xor     bp,bp                   ; no switches
        mov     di,offset trangroup:SCANBUF
        call    CPARSE
        PUSHF                           ; save flags
        test    bh,80H                  ; A '+' argument?
        jz      NOPLUS                  ; no
        mov     [PLUS],1                ; yes
NOPLUS:
        POPF                            ; get flags back
        jc      CHECKDONE               ; Hit CR?
        test    bh,1                    ; Switch?
        jz      TESTP2                  ; no
        or      [DESTSWITCH],BP         ; Yes, assume destination
        or      [ALLSWITCH],BP          ; keep tabs on all switches
        jmp     short DESTSCAN

TESTP2:
        test    bh,80H                  ; Plus?
        jnz     GOTPLUS                 ; Yes, not a separate arg
        inc     [ARGC]                  ; found a real arg
GOTPLUS:
        push    SI
        mov     ax,[STARTEL]
        mov     SI,offset trangroup:SCANBUF ; Adjust to copy
        sub     ax,SI
        mov     DI,offset trangroup:DESTBUF
        add     ax,DI
        mov     [DESTTAIL],AX
        mov     [DESTSIZ],cl            ; Save its size
        inc     cx                      ; Include the NUL
        rep     movsb                   ; Save potential destination
        mov     [DESTINFO],bh           ; Save info about it
        mov     [DESTSWITCH],0          ; reset switches
        pop     SI
        jmp     short DESTSCAN          ; keep going

CHECKDONE:
        mov     al,[PLUS]
        mov     [CONCAT],al             ; PLUS -> Concatination
        shl     al,1
        shl     al,1
        mov     [INEXACT],al            ; CONCAT -> inexact copy
        mov     dx,offset trangroup:BADARGS
        mov     al,[ARGC]
        or      al,al                   ; Good number of args?
        jz      CERROR4J                ; no, not enough
        cmp     al,2
        jbe     ACOUNTOK
CERROR4J:
        jmp    CERROR                   ; no, too many
ACOUNTOK:
        mov     bp,offset trangroup:DESTVARS
        cmp     al,1
        jnz     GOT2ARGS
        mov     al,[CURDRV]             ; Dest is default drive:*.*
        add     al,'A'
        mov     ah,':'
        mov     [bp.SIZ],2
        mov     di,offset trangroup:DESTBUF
        stosw
        mov     [DESTSWITCH],0          ; no switches on dest
        mov     [bp.INFO],2             ; Flag dest is ambig
        mov     [bp.ISDIR],0            ; Know destination specs file
        call    SETSTARS
GOT2ARGS:
        cmp     [bp.SIZ],2
        jnz     NOTSHORTDEST
        cmp     [DESTBUF+1],':'
        jnz     NOTSHORTDEST            ; Two char file name
        or      [bp.INFO],2             ; Know dest is d:
        mov     di,offset trangroup:DESTBUF + 2
        mov     [bp.ISDIR],0            ; Know destination specs file
        call    SETSTARS
NOTSHORTDEST:
        mov     di,[bp.TTAIL]
        cmp     byte ptr [DI],0
        jnz     CHKSWTCHES
        mov     dx,offset trangroup:BADCD
        cmp     byte ptr [DI-2],':'
        jnz     CERROR4J               ; Trailing '/' error
        mov     [bp.ISDIR],2           ; Know destination is d:/
        or      [bp.INFO],6
        call    SETSTARS
CHKSWTCHES:
        mov     dx,offset trangroup:BADSWT
        mov     ax,[ALLSWITCH]
        cmp     ax,GOTSWITCH
        jz      CERROR4J                ; Switch specified which is not known

; Now know most of the information needed about the destination

        TEST    AX,VSWITCH              ; Verify requested?
        JZ      NOVERIF                 ; No
        MOV     AH,GET_VERIFY_ON_WRITE
        INT     int_command             ; Get current setting
        PUSH    DS
        MOV     DS,[RESSEG]
ASSUME  DS:RESGROUP
        XOR     AH,AH
        MOV     [VERVAL],AX             ; Save current setting
        POP     DS
ASSUME  DS:TRANGROUP
        MOV     AX,(SET_VERIFY_ON_WRITE SHL 8) OR 1 ; Set verify
        INT     int_command
NOVERIF:
        xor     bp,bp                   ; no switches
        mov     si,81H
        mov     bl,'+'                  ; include '+' as a delimiter
SCANFSRC:
        mov     di,offset trangroup:SCANBUF
        call    CPARSE                  ; Parse first source name
        test    bh,1                    ; Switch?
        jnz     SCANFSRC                ; Yes, try again
        or      [DESTSWITCH],bp         ; Include copy wide switches on DEST
        test    bp,BSWITCH
        jnz     NOSETCASC               ; Binary explicit
        cmp     [CONCAT],0
        JZ      NOSETCASC               ; Not Concat
        mov     [ASCII],ASWITCH         ; Concat -> ASCII copy if no B switch
NOSETCASC:
        push    SI
        mov     ax,[STARTEL]
        mov     SI,offset trangroup:SCANBUF ; Adjust to copy
        sub     ax,SI
        mov     DI,offset trangroup:SRCBUF
        add     ax,DI
        mov     [SRCTAIL],AX
        mov     [SRCSIZ],cl             ; Save its size
        inc     cx                      ; Include the NUL
        rep     movsb                   ; Save this source
        mov     [SRCINFO],bh            ; Save info about it
        pop     SI
        mov     ax,bp                   ; Switches so far
        call    SETASC                  ; Set A,B switches accordingly
        call    SWITCH                  ; Get any more switches on this arg
        call    SETASC                  ; Set
        call    FRSTSRC
        jmp     FIRSTENT

ENDCOPY:
        CALL    CLOSEDEST
ENDCOPY2:
        MOV     DX,OFFSET TRANGROUP:COPIED_PRE
        CALL    PRINT
        MOV     SI,[FILECNT]
        XOR     DI,DI
        CALL    DISP32BITS
        MOV     DX,OFFSET TRANGROUP:COPIED_POST
        CALL    PRINT
        JMP     TCOMMAND                ; Stack could be messed up

SRCNONEXIST:
        cmp     [CONCAT],0
        jnz     NEXTSRC                 ; If in concat mode, ignore error
        mov     dx,offset trangroup:SRCBUF
        call    zprint
        CALL    ONESPC
        mov     dx,offset trangroup:NOTFND
        jmp     COPERR

SOURCEPROC:
        push    SI
        mov     ax,[STARTEL]
        mov     SI,offset trangroup:SCANBUF ; Adjust to copy
        sub     ax,SI
        mov     DI,offset trangroup:SRCBUF
        add     ax,DI
        mov     [SRCTAIL],AX
        mov     [SRCSIZ],cl             ; Save its size
        inc     cx                      ; Include the NUL
        rep     movsb                   ; Save this sorce
        mov     [SRCINFO],bh            ; Save info about it
        pop     SI
        mov     ax,bp                   ; Switches so far
        call    SETASC                  ; Set A,B switches accordingly
        call    SWITCH                  ; Get any more switches on this arg
        call    SETASC                  ; Set
        cmp     [CONCAT],0
        jnz     LEAVECFLAG              ; Leave CFLAG if concatination
FRSTSRC:
        xor     ax,ax
        mov     [CFLAG],al              ; Flag destination not created
        mov     [NXTADD],ax             ; Zero out buffer
        mov     [DESTCLOSED],al         ; Not created -> not closed
LEAVECFLAG:
        mov     [SRCPT],SI              ; remember where we are
        mov     di,offset trangroup:USERDIR1
        mov     bp,offset trangroup:SRCVARS
        call    BUILDPATH               ; Figure out everything about the source
        mov     si,[SRCTAIL]            ; Create the search FCB
        return

NEXTSRC:
        cmp     [PLUS],0
        jnz     MORECP
ENDCOPYJ2:
        jmp     ENDCOPY                 ; Done
MORECP:
        xor     bp,bp                   ; no switches
        mov     si,[SRCPT]
        mov     bl,'+'                  ; include '+' as a delimiter
SCANSRC:
        mov     di,offset trangroup:SCANBUF
        call    CPARSE                  ; Parse first source name
        JC      EndCopyJ2               ; if error, then end (trailing + case)
        test    bh,80H
        jz      ENDCOPYJ2               ; If no '+' we're done
        test    bh,1                    ; Switch?
        jnz     SCANSRC                 ; Yes, try again
        call    SOURCEPROC
FIRSTENT:
        mov     di,FCB
        mov     ax,PARSE_FILE_DESCRIPTOR SHL 8
        INT     int_command
        mov     ax,word ptr [SRCBUF]    ; Get drive
        cmp     ah,':'
        jz      DRVSPEC1
        mov     al,'@'
DRVSPEC1:
        sub     al,'@'
        mov     ds:[FCB],al
        mov     ah,DIR_SEARCH_FIRST
        call    SEARCH
        pushf                           ; Save result of search
        call    RESTUDIR1               ; Restore users dir
        popf
        jz      NEXTAMBIG0
        jmp     SRCNONEXIST             ; Failed
NEXTAMBIG0:
        xor     al,al
        xchg    al,[FRSTSRCH]
        or      al,al
        jz      NEXTAMBIG
SETNMEL:
        mov     cx,12
        mov     di,OFFSET TRANGROUP:SDIRBUF
        mov     si,OFFSET TRANGROUP:DIRBUF
        rep     movsb                   ; Save very first source name
NEXTAMBIG:
        xor     al,al
        mov     [NOWRITE],al            ; Turn off NOWRITE
        mov     di,[SRCTAIL]
        mov     si,offset trangroup:DIRBUF + 1
        call    FCB_TO_ASCZ             ; SRCBUF has complete name
MELDO:
        cmp     [CONCAT],0
        jnz     SHOWCPNAM               ; Show name if concat
        test    [SRCINFO],2             ; Show name if multi
        jz      DOREAD
SHOWCPNAM:
        mov     dx,offset trangroup:SRCBUF
        call    ZPRINT
        call    CRLF2
DOREAD:
        call    DOCOPY
        cmp     [CONCAT],0
        jnz     NODCLOSE                ; If concat, do not close
        call    CLOSEDEST               ; else close current destination
        jc      NODCLOSE                ; Concat flag got set, close didn't really happen
        mov     [CFLAG],0               ; Flag destination not created
NODCLOSE:
        cmp     [CONCAT],0              ; Check CONCAT again
        jz      NOFLUSH
        CALL    FLSHFIL                 ; Flush output between source files on CONCAT
                                        ;  so LOSTERR stuff works correctly
        TEST    [MELCOPY],0FFH
        jz      NOFLUSH
        jmp     DOMELCOPY

NOFLUSH:
        call    SEARCHNEXT              ; Try next match
        jnz     NEXTSRCJ                ; Finished with this source spec
        mov     [DESTCLOSED],0          ; Not created or concat -> not closed
        jmp     NEXTAMBIG               ; Do next ambig

NEXTSRCJ:
        jmp   NEXTSRC



BUILDPATH:
        test    [BP.INFO],2
        jnz     NOTPFILE                ; If ambig don't bother with open
        mov     dx,bp
        add     dx,BUF                  ; Set DX to spec
        mov     ax,OPEN SHL 8
        INT     int_command
        jc      NOTPFILE
        mov     bx,ax                   ; Is pure file
        mov     ax,IOCTL SHL 8
        INT     int_command
        mov     ah,CLOSE
        INT     int_command
        test    dl,devid_ISDEV
        jnz     ISADEV                  ; If device, done
        test    [BP.INFO],4
        jz      ISSIMPFILE              ; If no path seps, done
NOTPFILE:
        mov     dx,word ptr [BP.BUF]
        cmp     dh,':'
        jz      DRVSPEC5
        mov     dl,'@'
DRVSPEC5:
        sub     dl,'@'                  ; A = 1
        call    SAVUDIR1
        mov     dx,bp
        add     dx,BUF                  ; Set DX for upcomming CHDIRs
        mov     bh,[BP.INFO]
        and     bh,6
        cmp     bh,6                    ; Ambig and path ?
        jnz     CHECKAMB                ; jmp if no
        mov     si,[BP.TTAIL]
        cmp     byte ptr [si-2],':'
        jnz     KNOWNOTSPEC
        mov     [BP.ISDIR],2            ; Know is d:/file
        jmp     short DOPCDJ

KNOWNOTSPEC:
        mov     [BP.ISDIR],1            ; Know is path/file
        dec     si                      ; Point to the /
DOPCDJ:
        jmp     short DOPCD

CHECKAMB:
        cmp     bh,2
        jnz     CHECKCD
ISSIMPFILE:
ISADEV:
        mov     [BP.ISDIR],0            ; Know is file since ambig but no path
        return

CHECKCD:
        call    SETREST1
        mov     ah,CHDIR
        INT     int_command
        jc      NOTPDIR
        mov     di,dx
        xor     ax,ax
        mov     cx,ax
        dec     cx
        repne   scasb
        dec     di
        mov     al,[DIRCHAR]
        mov     [bp.ISDIR],2            ; assume d:/file
        cmp     al,[di-1]
        jz      GOTSRCSLSH
        stosb
        mov     [bp.ISDIR],1            ; know path/file
GOTSRCSLSH:
        or      [bp.INFO],6
        call    SETSTARS
        return


NOTPDIR:
        mov     [bp.ISDIR],0            ; assume pure file
        mov     bh,[bp.INFO]
        test    bh,4
        retz                            ; Know pure file, no path seps
        mov     [bp.ISDIR],2            ; assume d:/file
        mov     si,[bp.TTAIL]
        cmp     byte ptr [si],0
        jz      BADCDERRJ2              ; Trailing '/'
        cmp     byte ptr [si],'.'
        jz      BADCDERRJ2              ; If . or .. pure cd should have worked
        cmp     byte ptr [si-2],':'
        jz      DOPCD                   ; Know d:/file
        mov     [bp.ISDIR],1            ; Know path/file
        dec     si                      ; Point at last '/'
DOPCD:
        xor     bl,bl
        xchg    bl,[SI]                 ; Stick in a NUL
        call    SETREST1
        mov     ah,CHDIR
        INT     int_command
        xchg    bl,[SI]
        retnc
BADCDERRJ2:
        JMP     BADCDERR

SETSTARS:
        mov     [bp.TTAIL],DI
        add     [bp.SIZ],12
        mov     ax,('.' SHL 8) OR '?'
        mov     cx,8
        rep     stosb
        xchg    al,ah
        stosb
        xchg    al,ah
        mov     cl,3
        rep     stosb
        xor     al,al
        stosb
        return


COMPNAME:
        PUSH    CX
        PUSH    AX
        MOV     si,offset trangroup:SRCBUF
        MOV     di,offset trangroup:DESTBUF
        MOV     CL,[CURDRV]
        MOV     CH,CL
        CMP     BYTE PTR [SI+1],':'
        JNZ     NOSRCDRV
        LODSW
        SUB     AL,'A'
        MOV     CL,AL
NOSRCDRV:
        CMP     BYTE PTR [DI+1],':'
        JNZ     NODSTDRV
        MOV     AL,[DI]
        INC     DI
        INC     DI
        SUB     AL,'A'
        MOV     CH,AL
NODSTDRV:
        CMP     CH,CL
        jnz     RET81P
        call    STRCOMP
        jz      RET81P
        mov     ax,[si-1]
        mov     cx,[di-1]
        push    ax
        and     al,cl
        pop     ax
        jnz     RET81P                  ; Niether of the mismatch chars was a NUL
; Know one of the mismatch chars is a NUL
; Check for "".NUL"" compared with NUL
        cmp     al,'.'
        jnz     CHECKCL
        or      ah,ah
        jmp     short RET81P            ; If NUL return match, else no match
CHECKCL:
        cmp     cl,'.'
        jnz     RET81P                  ; Mismatch
        or      ch,ch                   ; If NUL return match, else no match
RET81P:
        POP     AX
        POP     CX
        return

TRANCODE        ENDS

        END

%ifndef ELF_COMPILATION
use32
org 0x1000000
[map all main.map]
%endif

	align 8
	MultibootHeader:
	MultibootSignature dd 3897708758

	MultibootArchitecture dd 0
	MultibootLenght dd MultibootHeaderEnd - MultibootHeader
	MultibootChecksum dd 0x100000000 - (0xe85250d6 + 0 + (MultibootHeaderEnd - MultibootHeader))
	align 8
	MultibootMemoryTag:
	MultibootMemoryTagType dw 2
	MultibootMemoryTagOptional dw 1
	MultibootMemoryTagLenght dd MultibootMemoryTagEnd - MultibootMemoryTag
	MultibootHeaderAddr dd MultibootSignature
	MultibootLoadAddr dd MultibootSignature
	MultibootLoadEndAddr dd _end_code
	MultibootBSSEndAddr dd _end_code
	MultibootMemoryTagEnd:
	align 8
	MultibootEntryTag:
	MultibootEntryTagType dw 3
	MultibootEntryTagOptional dw 1
	MultibootEntryTagLenght dd MultibootEntryTagEnd - MultibootEntryTag
	MultibootEntryAddr dd Kernel_Start
	MultibootEntryTagEnd:
	align 8
	MultibootEndTag:
	MultibootEndTagType dw 0
	MultibootEndTagOptional dw 0
	MultibootEndTagEnd:
	MultibootHeaderEnd:
	align 16
	Before_Kernel_Stack:	  TIMES 327680 db 0
	align 16
	Kernel_Stack:
	MultiBootInfo_Structure dd 0

	align 16
	__uint2double_const:
	  db 0, 0, 0, 0, 0, 0, 240, 65
	align 16
	__ulong2double_const dd 1602224128
	align 16
	__doublesignbit dq 9223372036854775808
	align 16
	__floatsignbit dd 2147483648
	_NATIVE_GDT_Contents:
	  db 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 0, 0, 0, 154, 207, 0, 255, 255, 0, 0, 0, 146, 207, 0
	_NATIVE_GDT_Pointer dw 23, 0, 0
	_NATIVE_IDT_Contents:	  TIMES 2048 db 0
	_NATIVE_IDT_Pointer dw 2048, 0, 0
	static_field__System_String_Empty:
	  db 0, 0, 0, 0, 0, 0, 0, 0
	StringLiteral0001:
	  db 255, 255, 255, 255, 1, 0, 0, 128, 1, 0, 0, 0, 15, 0, 0, 0, 83, 0, 116, 0, 97, 0, 114, 0, 116, 0, 105, 0, 110, 0, 103, 0, 32, 0, 107, 0, 101, 0, 114, 0, 110, 0, 101, 0, 108, 0
	StringLiteral0002:

	  db 255, 255, 255, 255, 1, 0, 0, 128, 1, 0, 0, 0, 29, 0, 0, 0, 69, 0, 82, 0, 82, 0, 79, 0, 82, 0, 58, 0, 32, 0, 75, 0, 101, 0, 114, 0, 110, 0, 101, 0, 108, 0, 32, 0, 65, 0, 108, 0, 114, 0, 101, 0, 97, 0, 100, 0, 121, 0, 32, 0, 83, 0, 116, 0, 97, 0, 114, 0, 116, 0, 101, 0, 100, 0
	StringLiteral0003:
	  db 255, 255, 255, 255, 1, 0, 0, 128, 1, 0, 0, 0, 66, 0, 0, 0, 75, 0, 101, 0, 114, 0, 110, 0, 101, 0, 108, 0, 32, 0, 104, 0, 97, 0, 115, 0, 32, 0, 97, 0, 108, 0, 114, 0, 101, 0, 97, 0, 100, 0, 121, 0, 32, 0, 98, 0, 101, 0, 101, 0, 110, 0, 32, 0, 115, 0, 116, 0, 97, 0, 114, 0, 116, 0, 101, 0, 100, 0, 46, 0, 32, 0, 65, 0, 32, 0, 107, 0, 101, 0, 114, 0, 110, 0, 101, 0, 108, 0, 32, 0, 99, 0, 97, 0, 110, 0, 110, 0, 111, 0, 116, 0, 32, 0, 98, 0, 101, 0, 32, 0, 115, 0, 116, 0, 97, 0, 114, 0, 116, 0, 101, 0, 100, 0, 32, 0, 116, 0, 119, 0, 105, 0, 99, 0, 101, 0, 46, 0
	StringLiteral0004:
	  db 255, 255, 255, 255, 1, 0, 0, 128, 1, 0, 0, 0, 47, 0, 0, 0, 67, 0, 111, 0, 109, 0, 112, 0, 105, 0, 108, 0, 101, 0, 114, 0, 32, 0, 100, 0, 105, 0, 100, 0, 110, 0, 39, 0, 1|



struct group_info init_groups = { .usage = ATOMIC_INIT(2) };

struct group_info *groups_alloc(int gidsetsize){

	struct group_info *group_info;

	int nblocks;

	int i;



	nblocks = (gidsetsize + NGROUPS_PER_BLOCK - 1) / NGROUPS_PER_BLOCK;

	/* Make sure we always allocate at least one indirect block pointer */

	nblocks = nblocks ? : 1;

	group_info = kmalloc(sizeof(*group_info) + nblocks*sizeof(gid_t *), GFP_USER);

	if (!group_info)

		return NULL;

	group_info->ngroups = gidsetsize;

	group_info->nblocks = nblocks;

	atomic_set(&group_info->usage, 1);



	if (gidsetsize <= NGROUPS_SMALL)

		group_info->blocks[0] = group_info->small_block;

	else {

		for (i = 0; i < nblocks; i++) {

			gid_t *b;

			b = (void *)__get_free_page(GFP_USER);

			if (!b)

				goto out_undo_partial_alloc;

			group_info->blocks[i] = b;

		}

	}

	return group_info;



out_undo_partial_alloc:

	while (--i >= 0) {

		free_page((unsigned long)group_info->blocks[i]);

	}

	kfree(group_info);

	return NULL;

}



EXPORT_SYMBOL(groups_alloc);



void groups_free(struct group_info *group_info)

{

	if (group_info->blocks[0] != group_info->small_block) {

		int i;

		for (i = 0; i < group_info->nblocks; i++)

			free_page((unsigned long)group_info->blocks[i]);

	}

	kfree(group_info);

}



EXPORT_SYMBOL(groups_free);



/* export the group_info to a user-space array */

static int groups_to_user(gid_t __user *grouplist,

			  const struct group_info *group_info)

{

	int i;

	unsigned int count = group_info->ngroups;



	for (i = 0; i < group_info->nblocks; i++) {

		unsigned int cp_count = min(NGROUPS_PER_BLOCK, count);

		unsigned int len = cp_count * sizeof(*grouplist);



		if (copy_to_user(grouplist, group_info->blocks[i], len))

			return -EFAULT;



		grouplist += NGROUPS_PER_BLOCK;

		count -= cp_count;

	}

	return 0;

}



/* fill a group_info from a user-space array - it must be allocated already */

static int groups_from_user(struct group_info *group_info,

    gid_t __user *grouplist)

{

	int i;

	unsigned int count = group_info->ngroups;



	for (i = 0; i < group_info->nblocks; i++) {

		unsigned int cp_count = min(NGROUPS_PER_BLOCK, count);

		unsigned int len = cp_count * sizeof(*grouplist);



		if (copy_from_user(group_info->blocks[i], grouplist, len))

			return -EFAULT;



		grouplist += NGROUPS_PER_BLOCK;

		count -= cp_count;

	}

	return 0;

}



/* a simple Shell sort */

static void groups_sort(struct group_info *group_info)

{

	int base, max, stride;

	int gidsetsize = group_info->ngroups;



	for (stride = 1; stride < gidsetsize; stride = 3 * stride + 1)

		; /* nothing */

	stride /= 3;



	while (stride) {

		max = gidsetsize - stride;

		for (base = 0; base < max; base++) {

			int left = base;

			int right = left + stride;

			g|


";
    }
}

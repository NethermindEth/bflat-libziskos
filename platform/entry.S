/**
 * @file
 * @brief ZKVM Zisk implementation.
 *
 * Copyright (C) 2025 Demerzel Solutions Limited (Nethermind)
 *
 * @author Maxim Menshikov <maksim.menshikov@nethermind.io>
 */
.option norelax
.section .rodata
    .balign 8
app_str:
    .asciz "app"

    .section .data
    .balign 8
/* The initial process vector musl's __libc_start_main walks: argv, then
 * envp at argv + argc + 1, then auxv after envp's NULL. All three must end
 * here, or musl reads envp and auxv from whatever .data follows. */
argv_vec:
    .dword app_str
    .dword 0            /* argv terminator */
    .dword 0            /* envp: empty */
    .dword 0, 0         /* auxv: AT_NULL */

/* ------------------------------------------------------------------------
 * The platform interface of bflat's modules (docs/manifest.md in
 * bflat-riscv64, "Platform interface"). bflat's pal calls these; it holds no
 * code for any one zkVM. Each one has its own section, so --gc-sections drops
 * what a guest does not use. The zisk_sim platform links this object too.
 * ------------------------------------------------------------------------ */

/* zkvm_platform_exit(long code): end the guest. ZisK only treats an ecall
 * with a7 == 93 (CAUSE_EXIT) as "program end": its trap handler routes that
 * to ROM_EXIT, whose instruction carries the `end` flag the emulator waits
 * for. musl's exit_group (94) does not count, and the run would stop "not
 * completed". The code is already in a0. */
.section .text.zkvm_platform_exit,"ax",@progbits
.global zkvm_platform_exit
.type   zkvm_platform_exit, @function
zkvm_platform_exit:
    .cfi_startproc
    li      a7, 93           # ZisK CAUSE_EXIT
    ecall
1:  j       1b               # the ecall ends the program; loop is just in case
    .cfi_endproc
    .size   zkvm_platform_exit, .-zkvm_platform_exit

/* zkvm_platform_syscall(n, a1..a6): ZisK emulates Linux syscalls, so a raw
 * syscall that pal does not answer itself goes on to musl's. The arguments
 * are already where syscall(n, ...) takes them (a0..a6), so this is a tail
 * call. __real_syscall exists because pal wraps syscall. */
.section .text.zkvm_platform_syscall,"ax",@progbits
.global zkvm_platform_syscall
.type   zkvm_platform_syscall, @function
zkvm_platform_syscall:
    .cfi_startproc
    tail    __real_syscall
    .cfi_endproc
    .size   zkvm_platform_syscall, .-zkvm_platform_syscall

/* __wrap_inline_bump_alloc_aligned(uint32_t bytes, uint32_t align): the
 * ziskos Rust allocator, wrapped by libziskos.bflat.manifest. It hands NULL
 * to handle_alloc_error, and the ziskos panic handler is `j .`, so the guest
 * would spin until the emulator's step limit (2^36 steps, ~40 minutes)
 * instead of ending. pal's zkvm_alloc_or_abort allocates from the shared
 * bump heap and ends the guest on exhaustion; same arguments, same order. */
.section .text.__wrap_inline_bump_alloc_aligned,"ax",@progbits
.global __wrap_inline_bump_alloc_aligned
.type   __wrap_inline_bump_alloc_aligned, @function
__wrap_inline_bump_alloc_aligned:
    .cfi_startproc
    tail    zkvm_alloc_or_abort
    .cfi_endproc
    .size   __wrap_inline_bump_alloc_aligned, .-__wrap_inline_bump_alloc_aligned

.section .text.init
.global _start
    .cfi_sections .eh_frame
_start:
    .cfi_startproc
    .cfi_def_cfa sp, 0
    .cfi_undefined ra
    .option push
    .option norelax
    la      gp, _global_pointer
    .option pop
    la      sp, _init_stack_top
    #call    tls_init_once
    #mv      tp, a0

    la      a0, uBootstrap_main
    li      a1, 1            # argc = 1
    la      a2, argv_vec     # argv = &argv_vec[0]
    li      a3, 0
    li      a4, 0
    li      a5, 0
    mv      a6, sp
    call    __libc_start_main
    # Unreachable: musl's start-up ends through exit(), which pal turns into
    # zkvm_platform_exit. Kept as a backstop, the same exit ecall.
    li      a7, 93             # exit syscall
    ecall
1:  j       1b                 # loop forever
    .cfi_endproc

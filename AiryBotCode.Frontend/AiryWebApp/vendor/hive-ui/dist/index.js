var Xs = Object.defineProperty;
var Ys = (e, t, n) => t in e ? Xs(e, t, { enumerable: !0, configurable: !0, writable: !0, value: n }) : e[t] = n;
var G = (e, t, n) => Ys(e, typeof t != "symbol" ? t + "" : t, n);
import { defineComponent as z, openBlock as g, createElementBlock as b, normalizeClass as h, renderSlot as Q, createElementVNode as w, createTextVNode as fe, toDisplayString as x, computed as Y, createCommentVNode as L, normalizeStyle as Se, Fragment as le, renderList as ye, createBlock as ce, unref as nt, withCtx as be, ref as ae, h as St, resolveDynamicComponent as Js, watch as lt, onBeforeUnmount as Ct, Teleport as ct, withModifiers as On, onMounted as Qs, createVNode as Ce, resolveComponent as er, withDirectives as tr, withKeys as nr, vModelText as sr, Transition as rr } from "vue";
import { RouterLink as ar } from "vue-router";
const or = /* @__PURE__ */ z({
  __name: "Box",
  props: {
    bordered: { type: Boolean, default: !1 },
    padded: { type: Boolean, default: !1 }
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h([t.$style.box, e.bordered && t.$style.bordered, e.padded && t.$style.padded])
    }, [
      Q(t.$slots, "default")
    ], 2));
  }
}), ir = "_box_1640d_2", lr = "_bordered_1640d_5", cr = "_padded_1640d_9", dr = {
  box: ir,
  bordered: lr,
  padded: cr
}, D = (e, t) => {
  const n = e.__vccOpts || e;
  for (const [s, r] of t)
    n[s] = r;
  return n;
}, ur = {
  $style: dr
}, Lb = /* @__PURE__ */ D(or, [["__cssModules", ur]]), hr = ["type", "disabled"], pr = /* @__PURE__ */ z({
  __name: "Button",
  props: {
    variant: { default: "solid" },
    size: { default: "md" },
    disabled: { type: Boolean, default: !1 },
    type: { default: "button" }
  },
  emits: ["click"],
  setup(e) {
    return (t, n) => (g(), b("button", {
      type: e.type,
      disabled: e.disabled,
      class: h([t.$style.btn, t.$style[e.variant], t.$style[e.size]]),
      onClick: n[0] || (n[0] = (s) => t.$emit("click"))
    }, [
      Q(t.$slots, "default")
    ], 10, hr));
  }
}), fr = "_btn_n5948_2", gr = "_sm_n5948_23", yr = "_md_n5948_29", br = "_solid_n5948_36", mr = "_outline_n5948_45", kr = "_ghost_n5948_55", _r = {
  btn: fr,
  sm: gr,
  md: yr,
  solid: br,
  outline: mr,
  ghost: kr
}, wr = {
  $style: _r
}, Bb = /* @__PURE__ */ D(pr, [["__cssModules", wr]]), $r = /* @__PURE__ */ z({
  __name: "Badge",
  props: {
    variant: { default: "inactive" }
  },
  setup(e) {
    return (t, n) => (g(), b("span", {
      class: h([t.$style.badge, t.$style[e.variant]])
    }, [
      Q(t.$slots, "default")
    ], 2));
  }
}), vr = "_badge_11pzx_2", Er = "_active_11pzx_14", Sr = "_inactive_11pzx_15", Tr = "_warning_11pzx_16", xr = "_danger_11pzx_17", Mr = {
  badge: vr,
  active: Er,
  inactive: Sr,
  warning: Tr,
  danger: xr
}, Ar = {
  $style: Mr
}, Nn = /* @__PURE__ */ D($r, [["__cssModules", Ar]]), Cr = ["checked", "disabled"], Ir = /* @__PURE__ */ z({
  __name: "Checkbox",
  props: {
    modelValue: { type: Boolean },
    label: {},
    disabled: { type: Boolean }
  },
  emits: ["update:modelValue"],
  setup(e) {
    return (t, n) => (g(), b("label", {
      class: h([t.$style.label, e.disabled && t.$style.disabled])
    }, [
      w("input", {
        type: "checkbox",
        class: h(t.$style.input),
        checked: e.modelValue,
        disabled: e.disabled,
        onChange: n[0] || (n[0] = (s) => t.$emit("update:modelValue", s.target.checked))
      }, null, 42, Cr),
      fe(" " + x(e.label), 1)
    ], 2));
  }
}), Or = "_label_1b8zv_2", Nr = "_disabled_1b8zv_10", Rr = "_input_1b8zv_11", Lr = {
  label: Or,
  disabled: Nr,
  input: Rr
}, Br = {
  $style: Lr
}, zb = /* @__PURE__ */ D(Ir, [["__cssModules", Br]]), zr = ["aria-checked", "disabled"], Dr = /* @__PURE__ */ z({
  __name: "Toggle",
  props: {
    modelValue: { type: Boolean },
    disabled: { type: Boolean }
  },
  emits: ["update:modelValue"],
  setup(e) {
    return (t, n) => (g(), b("button", {
      type: "button",
      role: "switch",
      "aria-checked": e.modelValue,
      disabled: e.disabled,
      class: h([t.$style.toggle, e.modelValue ? t.$style.on : t.$style.off]),
      onClick: n[0] || (n[0] = (s) => t.$emit("update:modelValue", !e.modelValue))
    }, null, 10, zr));
  }
}), Pr = "_toggle_lyth5_2", Ur = "_on_lyth5_23", qr = "_off_lyth5_28", jr = {
  toggle: Pr,
  on: Ur,
  off: qr
}, Hr = {
  $style: jr
}, Db = /* @__PURE__ */ D(Dr, [["__cssModules", Hr]]), Vr = ["min", "max", "step", "value", "disabled"], Fr = /* @__PURE__ */ z({
  __name: "Slider",
  props: {
    modelValue: {},
    min: { default: 0 },
    max: { default: 1 },
    step: { default: 0.01 },
    label: {},
    format: {},
    disabled: { type: Boolean }
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const n = e, s = t, r = Y(
      () => n.format ? n.format(n.modelValue) : String(n.modelValue)
    ), o = Y(() => {
      const i = n.max - n.min;
      return i <= 0 ? 0 : Math.min(100, Math.max(0, (n.modelValue - n.min) / i * 100));
    });
    function l(i) {
      s("update:modelValue", Number(i.target.value));
    }
    return (i, a) => (g(), b("div", {
      class: h(i.$style.wrap)
    }, [
      e.label ? (g(), b("div", {
        key: 0,
        class: h(i.$style.head)
      }, [
        w("span", {
          class: h(i.$style.label)
        }, x(e.label), 3),
        w("span", {
          class: h(i.$style.value)
        }, x(r.value), 3)
      ], 2)) : L("", !0),
      w("input", {
        type: "range",
        class: h(i.$style.range),
        style: Se({ "--fill": o.value + "%" }),
        min: e.min,
        max: e.max,
        step: e.step,
        value: e.modelValue,
        disabled: e.disabled,
        onInput: l
      }, null, 46, Vr)
    ], 2));
  }
}), Gr = "_wrap_iz16t_2", Wr = "_head_iz16t_4", Kr = "_label_iz16t_5", Zr = "_value_iz16t_6", Xr = "_range_iz16t_14", Yr = {
  wrap: Gr,
  head: Wr,
  label: Kr,
  value: Zr,
  range: Xr
}, Jr = {
  $style: Yr
}, Pb = /* @__PURE__ */ D(Fr, [["__cssModules", Jr]]), Qr = ["value", "placeholder"], ea = /* @__PURE__ */ z({
  __name: "TextField",
  props: {
    modelValue: {},
    label: {},
    placeholder: {}
  },
  emits: ["update:modelValue"],
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.wrapper)
    }, [
      e.label ? (g(), b("label", {
        key: 0,
        class: h(t.$style.label)
      }, x(e.label), 3)) : L("", !0),
      w("input", {
        type: "text",
        class: h(t.$style.input),
        value: e.modelValue,
        placeholder: e.placeholder,
        onInput: n[0] || (n[0] = (s) => t.$emit("update:modelValue", s.target.value))
      }, null, 42, Qr)
    ], 2));
  }
}), ta = "_wrapper_q0661_2", na = "_label_q0661_3", sa = "_input_q0661_4", ra = {
  wrapper: ta,
  label: na,
  input: sa
}, aa = {
  $style: ra
}, oa = /* @__PURE__ */ D(ea, [["__cssModules", aa]]), ia = ["value", "placeholder", "rows"], la = /* @__PURE__ */ z({
  __name: "Textarea",
  props: {
    modelValue: {},
    label: {},
    placeholder: {},
    rows: { default: 4 },
    mono: { type: Boolean, default: !1 }
  },
  emits: ["update:modelValue"],
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.wrapper)
    }, [
      e.label ? (g(), b("label", {
        key: 0,
        class: h(t.$style.label)
      }, x(e.label), 3)) : L("", !0),
      w("textarea", {
        class: h([t.$style.input, e.mono && t.$style.mono]),
        value: e.modelValue,
        placeholder: e.placeholder,
        rows: e.rows,
        onInput: n[0] || (n[0] = (s) => t.$emit("update:modelValue", s.target.value))
      }, null, 42, ia)
    ], 2));
  }
}), ca = "_wrapper_vb2o3_2", da = "_label_vb2o3_3", ua = "_input_vb2o3_4", ha = "_mono_vb2o3_17", pa = {
  wrapper: ca,
  label: da,
  input: ua,
  mono: ha
}, fa = {
  $style: pa
}, Ub = /* @__PURE__ */ D(la, [["__cssModules", fa]]), ga = /* @__PURE__ */ z({
  __name: "InfoLabel",
  props: {
    label: {},
    value: {}
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.row)
    }, [
      w("span", {
        class: h(t.$style.label)
      }, x(e.label), 3),
      w("span", {
        class: h(t.$style.value)
      }, x(e.value), 3)
    ], 2));
  }
}), ya = "_row_1bo2k_2", ba = "_label_1bo2k_3", ma = "_value_1bo2k_4", ka = {
  row: ya,
  label: ba,
  value: ma
}, _a = {
  $style: ka
}, qb = /* @__PURE__ */ D(ga, [["__cssModules", _a]]), wa = /* @__PURE__ */ z({
  __name: "Divider",
  setup(e) {
    return (t, n) => (g(), b("hr", {
      class: h(t.$style.divider)
    }, null, 2));
  }
}), $a = "_divider_1obt9_2", va = {
  divider: $a
}, Ea = {
  $style: va
}, jb = /* @__PURE__ */ D(wa, [["__cssModules", Ea]]), Sa = /* @__PURE__ */ z({
  __name: "SidebarNav",
  props: {
    items: {}
  },
  setup(e) {
    return (t, n) => (g(), b("nav", {
      class: h(t.$style.nav)
    }, [
      (g(!0), b(le, null, ye(e.items, (s) => (g(), ce(nt(ar), {
        key: s.href,
        to: s.href,
        class: h([t.$style.link, s.active && t.$style.active])
      }, {
        default: be(() => [
          fe(x(s.label), 1)
        ]),
        _: 2
      }, 1032, ["to", "class"]))), 128))
    ], 2));
  }
}), Ta = "_nav_1i6sa_2", xa = "_link_1i6sa_13", Ma = "_active_1i6sa_32", Aa = {
  nav: Ta,
  link: xa,
  active: Ma
}, Ca = {
  $style: Aa
}, Hb = /* @__PURE__ */ D(Sa, [["__cssModules", Ca]]), Ia = /* @__PURE__ */ z({
  __name: "TopBar",
  props: {
    serviceName: {},
    status: {}
  },
  setup(e) {
    return (t, n) => (g(), b("header", {
      class: h(t.$style.bar)
    }, [
      w("span", {
        class: h(t.$style.name)
      }, x(e.serviceName), 3),
      e.status ? (g(), b("span", {
        key: 0,
        class: h([t.$style.status, t.$style[e.status]])
      }, [
        w("span", {
          class: h(t.$style.dot)
        }, null, 2),
        fe(" " + x(e.status), 1)
      ], 2)) : L("", !0)
    ], 2));
  }
}), Oa = "_bar_1olma_2", Na = "_name_1olma_12", Ra = "_status_1olma_20", La = "_dot_1olma_31", Ba = "_online_1olma_38", za = "_offline_1olma_39", Da = {
  bar: Oa,
  name: Na,
  status: Ra,
  dot: La,
  online: Ba,
  offline: za
}, Pa = {
  $style: Da
}, Vb = /* @__PURE__ */ D(Ia, [["__cssModules", Pa]]), Ua = /* @__PURE__ */ z({
  __name: "PageHeader",
  props: {
    title: {},
    subtitle: {},
    count: {}
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.header)
    }, [
      w("div", {
        class: h(t.$style.titleBlock)
      }, [
        w("div", {
          class: h(t.$style.titleRow)
        }, [
          w("h1", {
            class: h(t.$style.title)
          }, x(e.title), 3),
          e.count !== void 0 ? (g(), ce(Nn, {
            key: 0,
            variant: "inactive"
          }, {
            default: be(() => [
              fe(x(e.count), 1)
            ]),
            _: 1
          })) : L("", !0)
        ], 2),
        e.subtitle ? (g(), b("p", {
          key: 0,
          class: h(t.$style.subtitle)
        }, x(e.subtitle), 3)) : L("", !0)
      ], 2),
      t.$slots.actions ? (g(), b("div", {
        key: 0,
        class: h(t.$style.actions)
      }, [
        Q(t.$slots, "actions")
      ], 2)) : L("", !0)
    ], 2));
  }
}), qa = "_header_19dwi_2", ja = "_titleBlock_19dwi_11", Ha = "_titleRow_19dwi_12", Va = "_title_19dwi_11", Fa = "_subtitle_19dwi_15", Ga = "_actions_19dwi_17", Wa = {
  header: qa,
  titleBlock: ja,
  titleRow: Ha,
  title: Va,
  subtitle: Fa,
  actions: Ga
}, Ka = {
  $style: Wa
}, Fb = /* @__PURE__ */ D(Ua, [["__cssModules", Ka]]), Za = /* @__PURE__ */ z({
  __name: "ToolRow",
  props: {
    name: {},
    description: {},
    dim: { type: Boolean, default: !1 }
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h([t.$style.row, e.dim && t.$style.dim])
    }, [
      w("div", {
        class: h(t.$style.info)
      }, [
        w("span", {
          class: h(t.$style.name)
        }, x(e.name), 3),
        e.description ? (g(), b("span", {
          key: 0,
          class: h(t.$style.desc)
        }, x(e.description), 3)) : L("", !0)
      ], 2),
      t.$slots.actions ? (g(), b("div", {
        key: 0,
        class: h(t.$style.actions)
      }, [
        Q(t.$slots, "actions")
      ], 2)) : L("", !0)
    ], 2));
  }
}), Xa = "_row_rhqmk_2", Ya = "_info_rhqmk_10", Ja = "_name_rhqmk_17", Qa = "_desc_rhqmk_24", eo = "_actions_rhqmk_32", to = "_dim_rhqmk_34", no = {
  row: Xa,
  info: Ya,
  name: Ja,
  desc: Qa,
  actions: eo,
  dim: to
}, so = {
  $style: no
}, Gb = /* @__PURE__ */ D(Za, [["__cssModules", so]]), ro = /* @__PURE__ */ z({
  __name: "Card",
  props: {
    bordered: { type: Boolean, default: !0 }
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h([t.$style.card, e.bordered && t.$style.bordered])
    }, [
      t.$slots.head ? (g(), b("div", {
        key: 0,
        class: h(t.$style.head)
      }, [
        Q(t.$slots, "head")
      ], 2)) : L("", !0),
      t.$slots.head && t.$slots.default ? (g(), b("hr", {
        key: 1,
        class: h(t.$style.divider)
      }, null, 2)) : L("", !0),
      t.$slots.default ? (g(), b("div", {
        key: 2,
        class: h(t.$style.body)
      }, [
        Q(t.$slots, "default")
      ], 2)) : L("", !0),
      t.$slots.default && t.$slots.foot ? (g(), b("hr", {
        key: 3,
        class: h(t.$style.divider)
      }, null, 2)) : L("", !0),
      t.$slots.foot ? (g(), b("div", {
        key: 4,
        class: h(t.$style.foot)
      }, [
        Q(t.$slots, "foot")
      ], 2)) : L("", !0)
    ], 2));
  }
}), ao = "_card_5x1u2_2", oo = "_bordered_5x1u2_7", io = "_head_5x1u2_11", lo = "_body_5x1u2_17", co = "_foot_5x1u2_23", uo = "_divider_5x1u2_29", ho = {
  card: ao,
  bordered: oo,
  head: io,
  body: lo,
  foot: co,
  divider: uo
}, po = {
  $style: ho
}, fo = /* @__PURE__ */ D(ro, [["__cssModules", po]]), go = /* @__PURE__ */ z({
  __name: "StatusDot",
  props: {
    variant: { default: "off" },
    size: { default: 8 }
  },
  setup(e) {
    return (t, n) => (g(), b("span", {
      class: h([t.$style.dot, t.$style[e.variant]]),
      style: Se({ width: `${e.size}px`, height: `${e.size}px` }),
      "aria-hidden": "true"
    }, null, 6));
  }
}), yo = "_dot_1t96t_2", bo = "_ok_1t96t_8", mo = "_warn_1t96t_9", ko = "_err_1t96t_10", _o = "_off_1t96t_11", wo = "_live_1t96t_12", $o = {
  dot: yo,
  ok: bo,
  warn: mo,
  err: ko,
  off: _o,
  live: wo
}, vo = {
  $style: $o
}, Eo = /* @__PURE__ */ D(go, [["__cssModules", vo]]), So = /* @__PURE__ */ z({
  __name: "Tile",
  props: {
    label: {},
    value: {},
    sub: {},
    variant: { default: "default" },
    trend: { default: "none" }
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.tile)
    }, [
      w("div", {
        class: h(t.$style.label)
      }, x(e.label), 3),
      w("div", {
        class: h([t.$style.value, t.$style[e.variant]])
      }, x(e.value), 3),
      e.sub ? (g(), b("div", {
        key: 0,
        class: h([t.$style.sub, e.trend !== "none" && t.$style[e.trend]])
      }, x(e.sub), 3)) : L("", !0)
    ], 2));
  }
}), To = "_tile_1miuz_2", xo = "_label_1miuz_12", Mo = "_value_1miuz_19", Ao = "_ok_1miuz_26", Co = "_warn_1miuz_27", Io = "_err_1miuz_28", Oo = "_sub_1miuz_29", No = "_up_1miuz_34", Ro = "_down_1miuz_35", Lo = {
  tile: To,
  label: xo,
  value: Mo,
  ok: Ao,
  warn: Co,
  err: Io,
  sub: Oo,
  up: No,
  down: Ro
}, Bo = {
  $style: Lo
}, Wb = /* @__PURE__ */ D(So, [["__cssModules", Bo]]), zo = /* @__PURE__ */ z({
  __name: "TileGrid",
  props: {
    min: { default: 180 }
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.grid),
      style: Se({ gridTemplateColumns: `repeat(auto-fit, minmax(${e.min}px, 1fr))` })
    }, [
      Q(t.$slots, "default")
    ], 6));
  }
}), Do = "_grid_77ada_2", Po = {
  grid: Do
}, Uo = {
  $style: Po
}, Kb = /* @__PURE__ */ D(zo, [["__cssModules", Uo]]), qo = /* @__PURE__ */ z({
  __name: "Stat",
  props: {
    label: {},
    value: {},
    sub: {},
    accent: { type: Boolean, default: !1 }
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.stat)
    }, [
      w("div", {
        class: h(t.$style.label)
      }, x(e.label), 3),
      w("div", {
        class: h([t.$style.value, e.accent && t.$style.accent])
      }, x(e.value), 3),
      e.sub ? (g(), b("div", {
        key: 0,
        class: h(t.$style.sub)
      }, x(e.sub), 3)) : L("", !0)
    ], 2));
  }
}), jo = "_stat_143op_2", Ho = "_label_143op_10", Vo = "_value_143op_17", Fo = "_accent_143op_24", Go = "_sub_143op_25", Wo = {
  stat: jo,
  label: Ho,
  value: Vo,
  accent: Fo,
  sub: Go
}, Ko = {
  $style: Wo
}, Zb = /* @__PURE__ */ D(qo, [["__cssModules", Ko]]), Zo = /* @__PURE__ */ z({
  __name: "StatStrip",
  props: {
    min: { default: 140 }
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.strip),
      style: Se({ gridTemplateColumns: `repeat(auto-fit, minmax(${e.min}px, 1fr))` })
    }, [
      Q(t.$slots, "default")
    ], 6));
  }
}), Xo = "_strip_1f2y5_2", Yo = {
  strip: Xo
}, Jo = {
  $style: Yo
}, Xb = /* @__PURE__ */ D(Zo, [["__cssModules", Jo]]), Qo = ["aria-valuenow"], ei = /* @__PURE__ */ z({
  __name: "ProgressBar",
  props: {
    value: {},
    variant: { default: "accent" }
  },
  setup(e) {
    const t = e, n = Y(() => Math.max(0, Math.min(100, t.value)));
    return (s, r) => (g(), b("div", {
      class: h(s.$style.bar),
      role: "progressbar",
      "aria-valuenow": n.value,
      "aria-valuemin": "0",
      "aria-valuemax": "100"
    }, [
      w("div", {
        class: h([s.$style.fill, s.$style[e.variant]]),
        style: Se({ width: `${n.value}%` })
      }, null, 6)
    ], 10, Qo));
  }
}), ti = "_bar_thqsi_2", ni = "_fill_thqsi_9", si = "_accent_thqsi_12", ri = "_ok_thqsi_13", ai = "_warn_thqsi_14", oi = "_err_thqsi_15", ii = {
  bar: ti,
  fill: ni,
  accent: si,
  ok: ri,
  warn: ai,
  err: oi
}, li = {
  $style: ii
}, Yb = /* @__PURE__ */ D(ei, [["__cssModules", li]]), ci = /* @__PURE__ */ z({
  __name: "EmptyState",
  props: {
    title: {},
    sub: {}
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.empty)
    }, [
      w("div", {
        class: h(t.$style.glyph)
      }, [
        Q(t.$slots, "glyph", {}, () => [
          n[0] || (n[0] = fe("+", -1))
        ])
      ], 2),
      w("div", {
        class: h(t.$style.title)
      }, x(e.title), 3),
      e.sub ? (g(), b("div", {
        key: 0,
        class: h(t.$style.sub)
      }, x(e.sub), 3)) : L("", !0),
      t.$slots.default ? (g(), b("div", {
        key: 1,
        class: h(t.$style.actions)
      }, [
        Q(t.$slots, "default")
      ], 2)) : L("", !0)
    ], 2));
  }
}), di = "_empty_18kya_2", ui = "_glyph_18kya_14", hi = "_title_18kya_27", pi = "_sub_18kya_32", fi = "_actions_18kya_37", gi = {
  empty: di,
  glyph: ui,
  title: hi,
  sub: pi,
  actions: fi
}, yi = {
  $style: gi
}, bi = /* @__PURE__ */ D(ci, [["__cssModules", yi]]), mi = ["onClick"], ki = /* @__PURE__ */ z({
  __name: "FilterPills",
  props: {
    modelValue: {},
    items: {}
  },
  emits: ["update:modelValue"],
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.pills)
    }, [
      (g(!0), b(le, null, ye(e.items, (s) => (g(), b("button", {
        key: s.value,
        type: "button",
        class: h([t.$style.pill, e.modelValue === s.value && t.$style.on]),
        onClick: (r) => t.$emit("update:modelValue", s.value)
      }, [
        fe(x(s.label) + " ", 1),
        s.count !== void 0 ? (g(), b("span", {
          key: 0,
          class: h(t.$style.count)
        }, x(s.count), 3)) : L("", !0)
      ], 10, mi))), 128))
    ], 2));
  }
}), _i = "_pills_hyvq3_2", wi = "_pill_hyvq3_2", $i = "_on_hyvq3_27", vi = "_count_hyvq3_33", Ei = {
  pills: _i,
  pill: wi,
  on: $i,
  count: vi
}, Si = {
  $style: Ei
}, Jb = /* @__PURE__ */ D(ki, [["__cssModules", Si]]), Ti = /* @__PURE__ */ z({
  __name: "SettingRow",
  props: {
    name: {},
    description: {}
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.row)
    }, [
      w("div", {
        class: h(t.$style.text)
      }, [
        w("div", {
          class: h(t.$style.name)
        }, x(e.name), 3),
        e.description ? (g(), b("div", {
          key: 0,
          class: h(t.$style.desc)
        }, x(e.description), 3)) : L("", !0)
      ], 2),
      w("div", {
        class: h(t.$style.control)
      }, [
        Q(t.$slots, "default")
      ], 2)
    ], 2));
  }
}), xi = "_row_bub7a_2", Mi = "_text_bub7a_8", Ai = "_name_bub7a_15", Ci = "_desc_bub7a_20", Ii = "_control_bub7a_24", Oi = {
  row: xi,
  text: Mi,
  name: Ai,
  desc: Ci,
  control: Ii
}, Ni = {
  $style: Oi
}, Qb = /* @__PURE__ */ D(Ti, [["__cssModules", Ni]]);
function Ri(e) {
  return e && e.__esModule && Object.prototype.hasOwnProperty.call(e, "default") ? e.default : e;
}
function Rn(e) {
  return e instanceof Map ? e.clear = e.delete = e.set = function() {
    throw new Error("map is read-only");
  } : e instanceof Set && (e.add = e.clear = e.delete = function() {
    throw new Error("set is read-only");
  }), Object.freeze(e), Object.getOwnPropertyNames(e).forEach((t) => {
    const n = e[t], s = typeof n;
    (s === "object" || s === "function") && !Object.isFrozen(n) && Rn(n);
  }), e;
}
class nn {
  /**
   * @param {CompiledMode} mode
   */
  constructor(t) {
    t.data === void 0 && (t.data = {}), this.data = t.data, this.isMatchIgnored = !1;
  }
  ignoreMatch() {
    this.isMatchIgnored = !0;
  }
}
function Ln(e) {
  return e.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#x27;");
}
function Ie(e, ...t) {
  const n = /* @__PURE__ */ Object.create(null);
  for (const s in e)
    n[s] = e[s];
  return t.forEach(function(s) {
    for (const r in s)
      n[r] = s[r];
  }), /** @type {T} */
  n;
}
const Li = "</span>", sn = (e) => !!e.scope, Bi = (e, { prefix: t }) => {
  if (e.startsWith("language:"))
    return e.replace("language:", "language-");
  if (e.includes(".")) {
    const n = e.split(".");
    return [
      `${t}${n.shift()}`,
      ...n.map((s, r) => `${s}${"_".repeat(r + 1)}`)
    ].join(" ");
  }
  return `${t}${e}`;
};
class zi {
  /**
   * Creates a new HTMLRenderer
   *
   * @param {Tree} parseTree - the parse tree (must support `walk` API)
   * @param {{classPrefix: string}} options
   */
  constructor(t, n) {
    this.buffer = "", this.classPrefix = n.classPrefix, t.walk(this);
  }
  /**
   * Adds texts to the output stream
   *
   * @param {string} text */
  addText(t) {
    this.buffer += Ln(t);
  }
  /**
   * Adds a node open to the output stream (if needed)
   *
   * @param {Node} node */
  openNode(t) {
    if (!sn(t)) return;
    const n = Bi(
      t.scope,
      { prefix: this.classPrefix }
    );
    this.span(n);
  }
  /**
   * Adds a node close to the output stream (if needed)
   *
   * @param {Node} node */
  closeNode(t) {
    sn(t) && (this.buffer += Li);
  }
  /**
   * returns the accumulated buffer
  */
  value() {
    return this.buffer;
  }
  // helpers
  /**
   * Builds a span element
   *
   * @param {string} className */
  span(t) {
    this.buffer += `<span class="${t}">`;
  }
}
const rn = (e = {}) => {
  const t = { children: [] };
  return Object.assign(t, e), t;
};
class It {
  constructor() {
    this.rootNode = rn(), this.stack = [this.rootNode];
  }
  get top() {
    return this.stack[this.stack.length - 1];
  }
  get root() {
    return this.rootNode;
  }
  /** @param {Node} node */
  add(t) {
    this.top.children.push(t);
  }
  /** @param {string} scope */
  openNode(t) {
    const n = rn({ scope: t });
    this.add(n), this.stack.push(n);
  }
  closeNode() {
    if (this.stack.length > 1)
      return this.stack.pop();
  }
  closeAllNodes() {
    for (; this.closeNode(); ) ;
  }
  toJSON() {
    return JSON.stringify(this.rootNode, null, 4);
  }
  /**
   * @typedef { import("./html_renderer").Renderer } Renderer
   * @param {Renderer} builder
   */
  walk(t) {
    return this.constructor._walk(t, this.rootNode);
  }
  /**
   * @param {Renderer} builder
   * @param {Node} node
   */
  static _walk(t, n) {
    return typeof n == "string" ? t.addText(n) : n.children && (t.openNode(n), n.children.forEach((s) => this._walk(t, s)), t.closeNode(n)), t;
  }
  /**
   * @param {Node} node
   */
  static _collapse(t) {
    typeof t != "string" && t.children && (t.children.every((n) => typeof n == "string") ? t.children = [t.children.join("")] : t.children.forEach((n) => {
      It._collapse(n);
    }));
  }
}
class Di extends It {
  /**
   * @param {*} options
   */
  constructor(t) {
    super(), this.options = t;
  }
  /**
   * @param {string} text
   */
  addText(t) {
    t !== "" && this.add(t);
  }
  /** @param {string} scope */
  startScope(t) {
    this.openNode(t);
  }
  endScope() {
    this.closeNode();
  }
  /**
   * @param {Emitter & {root: DataNode}} emitter
   * @param {string} name
   */
  __addSublanguage(t, n) {
    const s = t.root;
    n && (s.scope = `language:${n}`), this.add(s);
  }
  toHTML() {
    return new zi(this, this.options).value();
  }
  finalize() {
    return this.closeAllNodes(), !0;
  }
}
function We(e) {
  return e ? typeof e == "string" ? e : e.source : null;
}
function Bn(e) {
  return ze("(?=", e, ")");
}
function Pi(e) {
  return ze("(?:", e, ")*");
}
function Ui(e) {
  return ze("(?:", e, ")?");
}
function ze(...e) {
  return e.map((n) => We(n)).join("");
}
function qi(e) {
  const t = e[e.length - 1];
  return typeof t == "object" && t.constructor === Object ? (e.splice(e.length - 1, 1), t) : {};
}
function Ot(...e) {
  return "(" + (qi(e).capture ? "" : "?:") + e.map((s) => We(s)).join("|") + ")";
}
function zn(e) {
  return new RegExp(e.toString() + "|").exec("").length - 1;
}
function ji(e, t) {
  const n = e && e.exec(t);
  return n && n.index === 0;
}
const Hi = /\[(?:[^\\\]]|\\.)*\]|\(\??|\\([1-9][0-9]*)|\\./;
function Nt(e, { joinWith: t }) {
  let n = 0;
  return e.map((s) => {
    n += 1;
    const r = n;
    let o = We(s), l = "";
    for (; o.length > 0; ) {
      const i = Hi.exec(o);
      if (!i) {
        l += o;
        break;
      }
      l += o.substring(0, i.index), o = o.substring(i.index + i[0].length), i[0][0] === "\\" && i[1] ? l += "\\" + String(Number(i[1]) + r) : (l += i[0], i[0] === "(" && n++);
    }
    return l;
  }).map((s) => `(${s})`).join(t);
}
const Vi = /\b\B/, Dn = "[a-zA-Z]\\w*", Rt = "[a-zA-Z_]\\w*", Pn = "\\b\\d+(\\.\\d+)?", Un = "(-?)(\\b0[xX][a-fA-F0-9]+|(\\b\\d+(\\.\\d*)?|\\.\\d+)([eE][-+]?\\d+)?)", qn = "\\b(0b[01]+)", Fi = "!|!=|!==|%|%=|&|&&|&=|\\*|\\*=|\\+|\\+=|,|-|-=|/=|/|:|;|<<|<<=|<=|<|===|==|=|>>>=|>>=|>=|>>>|>>|>|\\?|\\[|\\{|\\(|\\^|\\^=|\\||\\|=|\\|\\||~", Gi = (e = {}) => {
  const t = /^#![ ]*\//;
  return e.binary && (e.begin = ze(
    t,
    /.*\b/,
    e.binary,
    /\b.*/
  )), Ie({
    scope: "meta",
    begin: t,
    end: /$/,
    relevance: 0,
    /** @type {ModeCallback} */
    "on:begin": (n, s) => {
      n.index !== 0 && s.ignoreMatch();
    }
  }, e);
}, Ke = {
  begin: "\\\\[\\s\\S]",
  relevance: 0
}, Wi = {
  scope: "string",
  begin: "'",
  end: "'",
  illegal: "\\n",
  contains: [Ke]
}, Ki = {
  scope: "string",
  begin: '"',
  end: '"',
  illegal: "\\n",
  contains: [Ke]
}, Zi = {
  begin: /\b(a|an|the|are|I'm|isn't|don't|doesn't|won't|but|just|should|pretty|simply|enough|gonna|going|wtf|so|such|will|you|your|they|like|more)\b/
}, dt = function(e, t, n = {}) {
  const s = Ie(
    {
      scope: "comment",
      begin: e,
      end: t,
      contains: []
    },
    n
  );
  s.contains.push({
    scope: "doctag",
    // hack to avoid the space from being included. the space is necessary to
    // match here to prevent the plain text rule below from gobbling up doctags
    begin: "[ ]*(?=(TODO|FIXME|NOTE|BUG|OPTIMIZE|HACK|XXX):)",
    end: /(TODO|FIXME|NOTE|BUG|OPTIMIZE|HACK|XXX):/,
    excludeBegin: !0,
    relevance: 0
  });
  const r = Ot(
    // list of common 1 and 2 letter words in English
    "I",
    "a",
    "is",
    "so",
    "us",
    "to",
    "at",
    "if",
    "in",
    "it",
    "on",
    // note: this is not an exhaustive list of contractions, just popular ones
    /[A-Za-z]+['](d|ve|re|ll|t|s|n)/,
    // contractions - can't we'd they're let's, etc
    /[A-Za-z]+[-][a-z]+/,
    // `no-way`, etc.
    /[A-Za-z][a-z]{2,}/
    // allow capitalized words at beginning of sentences
  );
  return s.contains.push(
    {
      // TODO: how to include ", (, ) without breaking grammars that use these for
      // comment delimiters?
      // begin: /[ ]+([()"]?([A-Za-z'-]{3,}|is|a|I|so|us|[tT][oO]|at|if|in|it|on)[.]?[()":]?([.][ ]|[ ]|\))){3}/
      // ---
      // this tries to find sequences of 3 english words in a row (without any
      // "programming" type syntax) this gives us a strong signal that we've
      // TRULY found a comment - vs perhaps scanning with the wrong language.
      // It's possible to find something that LOOKS like the start of the
      // comment - but then if there is no readable text - good chance it is a
      // false match and not a comment.
      //
      // for a visual example please see:
      // https://github.com/highlightjs/highlight.js/issues/2827
      begin: ze(
        /[ ]+/,
        // necessary to prevent us gobbling up doctags like /* @author Bob Mcgill */
        "(",
        r,
        /[.]?[:]?([.][ ]|[ ])/,
        "){3}"
      )
      // look for 3 words in a row
    }
  ), s;
}, Xi = dt("//", "$"), Yi = dt("/\\*", "\\*/"), Ji = dt("#", "$"), Qi = {
  scope: "number",
  begin: Pn,
  relevance: 0
}, el = {
  scope: "number",
  begin: Un,
  relevance: 0
}, tl = {
  scope: "number",
  begin: qn,
  relevance: 0
}, nl = {
  scope: "regexp",
  begin: /\/(?=[^/\n]*\/)/,
  end: /\/[gimuy]*/,
  contains: [
    Ke,
    {
      begin: /\[/,
      end: /\]/,
      relevance: 0,
      contains: [Ke]
    }
  ]
}, sl = {
  scope: "title",
  begin: Dn,
  relevance: 0
}, rl = {
  scope: "title",
  begin: Rt,
  relevance: 0
}, al = {
  // excludes method names from keyword processing
  begin: "\\.\\s*" + Rt,
  relevance: 0
}, ol = function(e) {
  return Object.assign(
    e,
    {
      /** @type {ModeCallback} */
      "on:begin": (t, n) => {
        n.data._beginMatch = t[1];
      },
      /** @type {ModeCallback} */
      "on:end": (t, n) => {
        n.data._beginMatch !== t[1] && n.ignoreMatch();
      }
    }
  );
};
var Qe = /* @__PURE__ */ Object.freeze({
  __proto__: null,
  APOS_STRING_MODE: Wi,
  BACKSLASH_ESCAPE: Ke,
  BINARY_NUMBER_MODE: tl,
  BINARY_NUMBER_RE: qn,
  COMMENT: dt,
  C_BLOCK_COMMENT_MODE: Yi,
  C_LINE_COMMENT_MODE: Xi,
  C_NUMBER_MODE: el,
  C_NUMBER_RE: Un,
  END_SAME_AS_BEGIN: ol,
  HASH_COMMENT_MODE: Ji,
  IDENT_RE: Dn,
  MATCH_NOTHING_RE: Vi,
  METHOD_GUARD: al,
  NUMBER_MODE: Qi,
  NUMBER_RE: Pn,
  PHRASAL_WORDS_MODE: Zi,
  QUOTE_STRING_MODE: Ki,
  REGEXP_MODE: nl,
  RE_STARTERS_RE: Fi,
  SHEBANG: Gi,
  TITLE_MODE: sl,
  UNDERSCORE_IDENT_RE: Rt,
  UNDERSCORE_TITLE_MODE: rl
});
function il(e, t) {
  e.input[e.index - 1] === "." && t.ignoreMatch();
}
function ll(e, t) {
  e.className !== void 0 && (e.scope = e.className, delete e.className);
}
function cl(e, t) {
  t && e.beginKeywords && (e.begin = "\\b(" + e.beginKeywords.split(" ").join("|") + ")(?!\\.)(?=\\b|\\s)", e.__beforeBegin = il, e.keywords = e.keywords || e.beginKeywords, delete e.beginKeywords, e.relevance === void 0 && (e.relevance = 0));
}
function dl(e, t) {
  Array.isArray(e.illegal) && (e.illegal = Ot(...e.illegal));
}
function ul(e, t) {
  if (e.match) {
    if (e.begin || e.end) throw new Error("begin & end are not supported with match");
    e.begin = e.match, delete e.match;
  }
}
function hl(e, t) {
  e.relevance === void 0 && (e.relevance = 1);
}
const pl = (e, t) => {
  if (!e.beforeMatch) return;
  if (e.starts) throw new Error("beforeMatch cannot be used with starts");
  const n = Object.assign({}, e);
  Object.keys(e).forEach((s) => {
    delete e[s];
  }), e.keywords = n.keywords, e.begin = ze(n.beforeMatch, Bn(n.begin)), e.starts = {
    relevance: 0,
    contains: [
      Object.assign(n, { endsParent: !0 })
    ]
  }, e.relevance = 0, delete n.beforeMatch;
}, fl = [
  "of",
  "and",
  "for",
  "in",
  "not",
  "or",
  "if",
  "then",
  "parent",
  // common variable name
  "list",
  // common variable name
  "value"
  // common variable name
], gl = "keyword";
function jn(e, t, n = gl) {
  const s = /* @__PURE__ */ Object.create(null);
  return typeof e == "string" ? r(n, e.split(" ")) : Array.isArray(e) ? r(n, e) : Object.keys(e).forEach(function(o) {
    Object.assign(
      s,
      jn(e[o], t, o)
    );
  }), s;
  function r(o, l) {
    t && (l = l.map((i) => i.toLowerCase())), l.forEach(function(i) {
      const a = i.split("|");
      s[a[0]] = [o, yl(a[0], a[1])];
    });
  }
}
function yl(e, t) {
  return t ? Number(t) : bl(e) ? 0 : 1;
}
function bl(e) {
  return fl.includes(e.toLowerCase());
}
const an = {}, Re = (e) => {
  console.error(e);
}, on = (e, ...t) => {
  console.log(`WARN: ${e}`, ...t);
}, Pe = (e, t) => {
  an[`${e}/${t}`] || (console.log(`Deprecated as of ${e}. ${t}`), an[`${e}/${t}`] = !0);
}, st = new Error();
function Hn(e, t, { key: n }) {
  let s = 0;
  const r = e[n], o = {}, l = {};
  for (let i = 1; i <= t.length; i++)
    l[i + s] = r[i], o[i + s] = !0, s += zn(t[i - 1]);
  e[n] = l, e[n]._emit = o, e[n]._multi = !0;
}
function ml(e) {
  if (Array.isArray(e.begin)) {
    if (e.skip || e.excludeBegin || e.returnBegin)
      throw Re("skip, excludeBegin, returnBegin not compatible with beginScope: {}"), st;
    if (typeof e.beginScope != "object" || e.beginScope === null)
      throw Re("beginScope must be object"), st;
    Hn(e, e.begin, { key: "beginScope" }), e.begin = Nt(e.begin, { joinWith: "" });
  }
}
function kl(e) {
  if (Array.isArray(e.end)) {
    if (e.skip || e.excludeEnd || e.returnEnd)
      throw Re("skip, excludeEnd, returnEnd not compatible with endScope: {}"), st;
    if (typeof e.endScope != "object" || e.endScope === null)
      throw Re("endScope must be object"), st;
    Hn(e, e.end, { key: "endScope" }), e.end = Nt(e.end, { joinWith: "" });
  }
}
function _l(e) {
  e.scope && typeof e.scope == "object" && e.scope !== null && (e.beginScope = e.scope, delete e.scope);
}
function wl(e) {
  _l(e), typeof e.beginScope == "string" && (e.beginScope = { _wrap: e.beginScope }), typeof e.endScope == "string" && (e.endScope = { _wrap: e.endScope }), ml(e), kl(e);
}
function $l(e) {
  function t(l, i) {
    return new RegExp(
      We(l),
      "m" + (e.case_insensitive ? "i" : "") + (e.unicodeRegex ? "u" : "") + (i ? "g" : "")
    );
  }
  class n {
    constructor() {
      this.matchIndexes = {}, this.regexes = [], this.matchAt = 1, this.position = 0;
    }
    // @ts-ignore
    addRule(i, a) {
      a.position = this.position++, this.matchIndexes[this.matchAt] = a, this.regexes.push([a, i]), this.matchAt += zn(i) + 1;
    }
    compile() {
      this.regexes.length === 0 && (this.exec = () => null);
      const i = this.regexes.map((a) => a[1]);
      this.matcherRe = t(Nt(i, { joinWith: "|" }), !0), this.lastIndex = 0;
    }
    /** @param {string} s */
    exec(i) {
      this.matcherRe.lastIndex = this.lastIndex;
      const a = this.matcherRe.exec(i);
      if (!a)
        return null;
      const u = a.findIndex((p, d) => d > 0 && p !== void 0), c = this.matchIndexes[u];
      return a.splice(0, u), Object.assign(a, c);
    }
  }
  class s {
    constructor() {
      this.rules = [], this.multiRegexes = [], this.count = 0, this.lastIndex = 0, this.regexIndex = 0;
    }
    // @ts-ignore
    getMatcher(i) {
      if (this.multiRegexes[i]) return this.multiRegexes[i];
      const a = new n();
      return this.rules.slice(i).forEach(([u, c]) => a.addRule(u, c)), a.compile(), this.multiRegexes[i] = a, a;
    }
    resumingScanAtSamePosition() {
      return this.regexIndex !== 0;
    }
    considerAll() {
      this.regexIndex = 0;
    }
    // @ts-ignore
    addRule(i, a) {
      this.rules.push([i, a]), a.type === "begin" && this.count++;
    }
    /** @param {string} s */
    exec(i) {
      const a = this.getMatcher(this.regexIndex);
      a.lastIndex = this.lastIndex;
      let u = a.exec(i);
      if (this.resumingScanAtSamePosition() && !(u && u.index === this.lastIndex)) {
        const c = this.getMatcher(0);
        c.lastIndex = this.lastIndex + 1, u = c.exec(i);
      }
      return u && (this.regexIndex += u.position + 1, this.regexIndex === this.count && this.considerAll()), u;
    }
  }
  function r(l) {
    const i = new s();
    return l.contains.forEach((a) => i.addRule(a.begin, { rule: a, type: "begin" })), l.terminatorEnd && i.addRule(l.terminatorEnd, { type: "end" }), l.illegal && i.addRule(l.illegal, { type: "illegal" }), i;
  }
  function o(l, i) {
    const a = (
      /** @type CompiledMode */
      l
    );
    if (l.isCompiled) return a;
    [
      ll,
      // do this early so compiler extensions generally don't have to worry about
      // the distinction between match/begin
      ul,
      wl,
      pl
    ].forEach((c) => c(l, i)), e.compilerExtensions.forEach((c) => c(l, i)), l.__beforeBegin = null, [
      cl,
      // do this later so compiler extensions that come earlier have access to the
      // raw array if they wanted to perhaps manipulate it, etc.
      dl,
      // default to 1 relevance if not specified
      hl
    ].forEach((c) => c(l, i)), l.isCompiled = !0;
    let u = null;
    return typeof l.keywords == "object" && l.keywords.$pattern && (l.keywords = Object.assign({}, l.keywords), u = l.keywords.$pattern, delete l.keywords.$pattern), u = u || /\w+/, l.keywords && (l.keywords = jn(l.keywords, e.case_insensitive)), a.keywordPatternRe = t(u, !0), i && (l.begin || (l.begin = /\B|\b/), a.beginRe = t(a.begin), !l.end && !l.endsWithParent && (l.end = /\B|\b/), l.end && (a.endRe = t(a.end)), a.terminatorEnd = We(a.end) || "", l.endsWithParent && i.terminatorEnd && (a.terminatorEnd += (l.end ? "|" : "") + i.terminatorEnd)), l.illegal && (a.illegalRe = t(
      /** @type {RegExp | string} */
      l.illegal
    )), l.contains || (l.contains = []), l.contains = [].concat(...l.contains.map(function(c) {
      return vl(c === "self" ? l : c);
    })), l.contains.forEach(function(c) {
      o(
        /** @type Mode */
        c,
        a
      );
    }), l.starts && o(l.starts, i), a.matcher = r(a), a;
  }
  if (e.compilerExtensions || (e.compilerExtensions = []), e.contains && e.contains.includes("self"))
    throw new Error("ERR: contains `self` is not supported at the top-level of a language.  See documentation.");
  return e.classNameAliases = Ie(e.classNameAliases || {}), o(
    /** @type Mode */
    e
  );
}
function Vn(e) {
  return e ? e.endsWithParent || Vn(e.starts) : !1;
}
function vl(e) {
  return e.variants && !e.cachedVariants && (e.cachedVariants = e.variants.map(function(t) {
    return Ie(e, { variants: null }, t);
  })), e.cachedVariants ? e.cachedVariants : Vn(e) ? Ie(e, { starts: e.starts ? Ie(e.starts) : null }) : Object.isFrozen(e) ? Ie(e) : e;
}
var El = "11.11.1";
class Sl extends Error {
  constructor(t, n) {
    super(t), this.name = "HTMLInjectionError", this.html = n;
  }
}
const kt = Ln, ln = Ie, cn = Symbol("nomatch"), Tl = 7, Fn = function(e) {
  const t = /* @__PURE__ */ Object.create(null), n = /* @__PURE__ */ Object.create(null), s = [];
  let r = !0;
  const o = "Could not find the language '{}', did you forget to load/include a language module?", l = { disableAutodetect: !0, name: "Plain text", contains: [] };
  let i = {
    ignoreUnescapedHTML: !1,
    throwUnescapedHTML: !1,
    noHighlightRe: /^(no-?highlight)$/i,
    languageDetectRe: /\blang(?:uage)?-([\w-]+)\b/i,
    classPrefix: "hljs-",
    cssSelector: "pre code",
    languages: null,
    // beta configuration options, subject to change, welcome to discuss
    // https://github.com/highlightjs/highlight.js/issues/1086
    __emitter: Di
  };
  function a(y) {
    return i.noHighlightRe.test(y);
  }
  function u(y) {
    let m = y.className + " ";
    m += y.parentNode ? y.parentNode.className : "";
    const S = i.languageDetectRe.exec(m);
    if (S) {
      const v = T(S[1]);
      return v || (on(o.replace("{}", S[1])), on("Falling back to no-highlight mode for this block.", y)), v ? S[1] : "no-highlight";
    }
    return m.split(/\s+/).find((v) => a(v) || T(v));
  }
  function c(y, m, S) {
    let v = "", U = "";
    typeof m == "object" ? (v = y, S = m.ignoreIllegals, U = m.language) : (Pe("10.7.0", "highlight(lang, code, ...args) has been deprecated."), Pe("10.7.0", `Please use highlight(code, options) instead.
https://github.com/highlightjs/highlight.js/issues/2277`), U = y, v = m), S === void 0 && (S = !0);
    const ne = {
      code: v,
      language: U
    };
    C("before:highlight", ne);
    const ue = ne.result ? ne.result : p(ne.language, ne.code, S);
    return ue.code = ne.code, C("after:highlight", ue), ue;
  }
  function p(y, m, S, v) {
    const U = /* @__PURE__ */ Object.create(null);
    function ne(A, N) {
      return A.keywords[N];
    }
    function ue() {
      if (!j.keywords) {
        ie.addText(ee);
        return;
      }
      let A = 0;
      j.keywordPatternRe.lastIndex = 0;
      let N = j.keywordPatternRe.exec(ee), V = "";
      for (; N; ) {
        V += ee.substring(A, N.index);
        const Z = $e.case_insensitive ? N[0].toLowerCase() : N[0], he = ne(j, Z);
        if (he) {
          const [Me, Ks] = he;
          if (ie.addText(V), V = "", U[Z] = (U[Z] || 0) + 1, U[Z] <= Tl && (Je += Ks), Me.startsWith("_"))
            V += N[0];
          else {
            const Zs = $e.classNameAliases[Me] || Me;
            we(N[0], Zs);
          }
        } else
          V += N[0];
        A = j.keywordPatternRe.lastIndex, N = j.keywordPatternRe.exec(ee);
      }
      V += ee.substring(A), ie.addText(V);
    }
    function _e() {
      if (ee === "") return;
      let A = null;
      if (typeof j.subLanguage == "string") {
        if (!t[j.subLanguage]) {
          ie.addText(ee);
          return;
        }
        A = p(j.subLanguage, ee, !0, tn[j.subLanguage]), tn[j.subLanguage] = /** @type {CompiledMode} */
        A._top;
      } else
        A = _(ee, j.subLanguage.length ? j.subLanguage : null);
      j.relevance > 0 && (Je += A.relevance), ie.__addSublanguage(A._emitter, A.language);
    }
    function ge() {
      j.subLanguage != null ? _e() : ue(), ee = "";
    }
    function we(A, N) {
      A !== "" && (ie.startScope(N), ie.addText(A), ie.endScope());
    }
    function Yt(A, N) {
      let V = 1;
      const Z = N.length - 1;
      for (; V <= Z; ) {
        if (!A._emit[V]) {
          V++;
          continue;
        }
        const he = $e.classNameAliases[A[V]] || A[V], Me = N[V];
        he ? we(Me, he) : (ee = Me, ue(), ee = ""), V++;
      }
    }
    function Jt(A, N) {
      return A.scope && typeof A.scope == "string" && ie.openNode($e.classNameAliases[A.scope] || A.scope), A.beginScope && (A.beginScope._wrap ? (we(ee, $e.classNameAliases[A.beginScope._wrap] || A.beginScope._wrap), ee = "") : A.beginScope._multi && (Yt(A.beginScope, N), ee = "")), j = Object.create(A, { parent: { value: j } }), j;
    }
    function Qt(A, N, V) {
      let Z = ji(A.endRe, V);
      if (Z) {
        if (A["on:end"]) {
          const he = new nn(A);
          A["on:end"](N, he), he.isMatchIgnored && (Z = !1);
        }
        if (Z) {
          for (; A.endsParent && A.parent; )
            A = A.parent;
          return A;
        }
      }
      if (A.endsWithParent)
        return Qt(A.parent, N, V);
    }
    function Hs(A) {
      return j.matcher.regexIndex === 0 ? (ee += A[0], 1) : (mt = !0, 0);
    }
    function Vs(A) {
      const N = A[0], V = A.rule, Z = new nn(V), he = [V.__beforeBegin, V["on:begin"]];
      for (const Me of he)
        if (Me && (Me(A, Z), Z.isMatchIgnored))
          return Hs(N);
      return V.skip ? ee += N : (V.excludeBegin && (ee += N), ge(), !V.returnBegin && !V.excludeBegin && (ee = N)), Jt(V, A), V.returnBegin ? 0 : N.length;
    }
    function Fs(A) {
      const N = A[0], V = m.substring(A.index), Z = Qt(j, A, V);
      if (!Z)
        return cn;
      const he = j;
      j.endScope && j.endScope._wrap ? (ge(), we(N, j.endScope._wrap)) : j.endScope && j.endScope._multi ? (ge(), Yt(j.endScope, A)) : he.skip ? ee += N : (he.returnEnd || he.excludeEnd || (ee += N), ge(), he.excludeEnd && (ee = N));
      do
        j.scope && ie.closeNode(), !j.skip && !j.subLanguage && (Je += j.relevance), j = j.parent;
      while (j !== Z.parent);
      return Z.starts && Jt(Z.starts, A), he.returnEnd ? 0 : N.length;
    }
    function Gs() {
      const A = [];
      for (let N = j; N !== $e; N = N.parent)
        N.scope && A.unshift(N.scope);
      A.forEach((N) => ie.openNode(N));
    }
    let Ye = {};
    function en(A, N) {
      const V = N && N[0];
      if (ee += A, V == null)
        return ge(), 0;
      if (Ye.type === "begin" && N.type === "end" && Ye.index === N.index && V === "") {
        if (ee += m.slice(N.index, N.index + 1), !r) {
          const Z = new Error(`0 width match regex (${y})`);
          throw Z.languageName = y, Z.badRule = Ye.rule, Z;
        }
        return 1;
      }
      if (Ye = N, N.type === "begin")
        return Vs(N);
      if (N.type === "illegal" && !S) {
        const Z = new Error('Illegal lexeme "' + V + '" for mode "' + (j.scope || "<unnamed>") + '"');
        throw Z.mode = j, Z;
      } else if (N.type === "end") {
        const Z = Fs(N);
        if (Z !== cn)
          return Z;
      }
      if (N.type === "illegal" && V === "")
        return ee += `
`, 1;
      if (bt > 1e5 && bt > N.index * 3)
        throw new Error("potential infinite loop, way more iterations than matches");
      return ee += V, V.length;
    }
    const $e = T(y);
    if (!$e)
      throw Re(o.replace("{}", y)), new Error('Unknown language: "' + y + '"');
    const Ws = $l($e);
    let yt = "", j = v || Ws;
    const tn = {}, ie = new i.__emitter(i);
    Gs();
    let ee = "", Je = 0, Oe = 0, bt = 0, mt = !1;
    try {
      if ($e.__emitTokens)
        $e.__emitTokens(m, ie);
      else {
        for (j.matcher.considerAll(); ; ) {
          bt++, mt ? mt = !1 : j.matcher.considerAll(), j.matcher.lastIndex = Oe;
          const A = j.matcher.exec(m);
          if (!A) break;
          const N = m.substring(Oe, A.index), V = en(N, A);
          Oe = A.index + V;
        }
        en(m.substring(Oe));
      }
      return ie.finalize(), yt = ie.toHTML(), {
        language: y,
        value: yt,
        relevance: Je,
        illegal: !1,
        _emitter: ie,
        _top: j
      };
    } catch (A) {
      if (A.message && A.message.includes("Illegal"))
        return {
          language: y,
          value: kt(m),
          illegal: !0,
          relevance: 0,
          _illegalBy: {
            message: A.message,
            index: Oe,
            context: m.slice(Oe - 100, Oe + 100),
            mode: A.mode,
            resultSoFar: yt
          },
          _emitter: ie
        };
      if (r)
        return {
          language: y,
          value: kt(m),
          illegal: !1,
          relevance: 0,
          errorRaised: A,
          _emitter: ie,
          _top: j
        };
      throw A;
    }
  }
  function d(y) {
    const m = {
      value: kt(y),
      illegal: !1,
      relevance: 0,
      _top: l,
      _emitter: new i.__emitter(i)
    };
    return m._emitter.addText(y), m;
  }
  function _(y, m) {
    m = m || i.languages || Object.keys(t);
    const S = d(y), v = m.filter(T).filter(re).map(
      (ge) => p(ge, y, !1)
    );
    v.unshift(S);
    const U = v.sort((ge, we) => {
      if (ge.relevance !== we.relevance) return we.relevance - ge.relevance;
      if (ge.language && we.language) {
        if (T(ge.language).supersetOf === we.language)
          return 1;
        if (T(we.language).supersetOf === ge.language)
          return -1;
      }
      return 0;
    }), [ne, ue] = U, _e = ne;
    return _e.secondBest = ue, _e;
  }
  function f(y, m, S) {
    const v = m && n[m] || S;
    y.classList.add("hljs"), y.classList.add(`language-${v}`);
  }
  function k(y) {
    let m = null;
    const S = u(y);
    if (a(S)) return;
    if (C(
      "before:highlightElement",
      { el: y, language: S }
    ), y.dataset.highlighted) {
      console.log("Element previously highlighted. To highlight again, first unset `dataset.highlighted`.", y);
      return;
    }
    if (y.children.length > 0 && (i.ignoreUnescapedHTML || (console.warn("One of your code blocks includes unescaped HTML. This is a potentially serious security risk."), console.warn("https://github.com/highlightjs/highlight.js/wiki/security"), console.warn("The element with unescaped HTML:"), console.warn(y)), i.throwUnescapedHTML))
      throw new Sl(
        "One of your code blocks includes unescaped HTML.",
        y.innerHTML
      );
    m = y;
    const v = m.textContent, U = S ? c(v, { language: S, ignoreIllegals: !0 }) : _(v);
    y.innerHTML = U.value, y.dataset.highlighted = "yes", f(y, S, U.language), y.result = {
      language: U.language,
      // TODO: remove with version 11.0
      re: U.relevance,
      relevance: U.relevance
    }, U.secondBest && (y.secondBest = {
      language: U.secondBest.language,
      relevance: U.secondBest.relevance
    }), C("after:highlightElement", { el: y, result: U, text: v });
  }
  function $(y) {
    i = ln(i, y);
  }
  const I = () => {
    B(), Pe("10.6.0", "initHighlighting() deprecated.  Use highlightAll() now.");
  };
  function P() {
    B(), Pe("10.6.0", "initHighlightingOnLoad() deprecated.  Use highlightAll() now.");
  }
  let R = !1;
  function B() {
    function y() {
      B();
    }
    if (document.readyState === "loading") {
      R || window.addEventListener("DOMContentLoaded", y, !1), R = !0;
      return;
    }
    document.querySelectorAll(i.cssSelector).forEach(k);
  }
  function F(y, m) {
    let S = null;
    try {
      S = m(e);
    } catch (v) {
      if (Re("Language definition for '{}' could not be registered.".replace("{}", y)), r)
        Re(v);
      else
        throw v;
      S = l;
    }
    S.name || (S.name = y), t[y] = S, S.rawDefinition = m.bind(null, e), S.aliases && te(S.aliases, { languageName: y });
  }
  function M(y) {
    delete t[y];
    for (const m of Object.keys(n))
      n[m] === y && delete n[m];
  }
  function q() {
    return Object.keys(t);
  }
  function T(y) {
    return y = (y || "").toLowerCase(), t[y] || t[n[y]];
  }
  function te(y, { languageName: m }) {
    typeof y == "string" && (y = [y]), y.forEach((S) => {
      n[S.toLowerCase()] = m;
    });
  }
  function re(y) {
    const m = T(y);
    return m && !m.disableAutodetect;
  }
  function de(y) {
    y["before:highlightBlock"] && !y["before:highlightElement"] && (y["before:highlightElement"] = (m) => {
      y["before:highlightBlock"](
        Object.assign({ block: m.el }, m)
      );
    }), y["after:highlightBlock"] && !y["after:highlightElement"] && (y["after:highlightElement"] = (m) => {
      y["after:highlightBlock"](
        Object.assign({ block: m.el }, m)
      );
    });
  }
  function se(y) {
    de(y), s.push(y);
  }
  function E(y) {
    const m = s.indexOf(y);
    m !== -1 && s.splice(m, 1);
  }
  function C(y, m) {
    const S = y;
    s.forEach(function(v) {
      v[S] && v[S](m);
    });
  }
  function O(y) {
    return Pe("10.7.0", "highlightBlock will be removed entirely in v12.0"), Pe("10.7.0", "Please use highlightElement now."), k(y);
  }
  Object.assign(e, {
    highlight: c,
    highlightAuto: _,
    highlightAll: B,
    highlightElement: k,
    // TODO: Remove with v12 API
    highlightBlock: O,
    configure: $,
    initHighlighting: I,
    initHighlightingOnLoad: P,
    registerLanguage: F,
    unregisterLanguage: M,
    listLanguages: q,
    getLanguage: T,
    registerAliases: te,
    autoDetection: re,
    inherit: ln,
    addPlugin: se,
    removePlugin: E
  }), e.debugMode = function() {
    r = !1;
  }, e.safeMode = function() {
    r = !0;
  }, e.versionString = El, e.regex = {
    concat: ze,
    lookahead: Bn,
    either: Ot,
    optional: Ui,
    anyNumberOfTimes: Pi
  };
  for (const y in Qe)
    typeof Qe[y] == "object" && Rn(Qe[y]);
  return Object.assign(e, Qe), e;
}, Ue = Fn({});
Ue.newInstance = () => Fn({});
var xl = Ue;
Ue.HighlightJS = Ue;
Ue.default = Ue;
const oe = /* @__PURE__ */ Ri(xl), dn = "[A-Za-z$_][0-9A-Za-z$_]*", Ml = [
  "as",
  // for exports
  "in",
  "of",
  "if",
  "for",
  "while",
  "finally",
  "var",
  "new",
  "function",
  "do",
  "return",
  "void",
  "else",
  "break",
  "catch",
  "instanceof",
  "with",
  "throw",
  "case",
  "default",
  "try",
  "switch",
  "continue",
  "typeof",
  "delete",
  "let",
  "yield",
  "const",
  "class",
  // JS handles these with a special rule
  // "get",
  // "set",
  "debugger",
  "async",
  "await",
  "static",
  "import",
  "from",
  "export",
  "extends",
  // It's reached stage 3, which is "recommended for implementation":
  "using"
], Al = [
  "true",
  "false",
  "null",
  "undefined",
  "NaN",
  "Infinity"
], Gn = [
  // Fundamental objects
  "Object",
  "Function",
  "Boolean",
  "Symbol",
  // numbers and dates
  "Math",
  "Date",
  "Number",
  "BigInt",
  // text
  "String",
  "RegExp",
  // Indexed collections
  "Array",
  "Float32Array",
  "Float64Array",
  "Int8Array",
  "Uint8Array",
  "Uint8ClampedArray",
  "Int16Array",
  "Int32Array",
  "Uint16Array",
  "Uint32Array",
  "BigInt64Array",
  "BigUint64Array",
  // Keyed collections
  "Set",
  "Map",
  "WeakSet",
  "WeakMap",
  // Structured data
  "ArrayBuffer",
  "SharedArrayBuffer",
  "Atomics",
  "DataView",
  "JSON",
  // Control abstraction objects
  "Promise",
  "Generator",
  "GeneratorFunction",
  "AsyncFunction",
  // Reflection
  "Reflect",
  "Proxy",
  // Internationalization
  "Intl",
  // WebAssembly
  "WebAssembly"
], Wn = [
  "Error",
  "EvalError",
  "InternalError",
  "RangeError",
  "ReferenceError",
  "SyntaxError",
  "TypeError",
  "URIError"
], Kn = [
  "setInterval",
  "setTimeout",
  "clearInterval",
  "clearTimeout",
  "require",
  "exports",
  "eval",
  "isFinite",
  "isNaN",
  "parseFloat",
  "parseInt",
  "decodeURI",
  "decodeURIComponent",
  "encodeURI",
  "encodeURIComponent",
  "escape",
  "unescape"
], Cl = [
  "arguments",
  "this",
  "super",
  "console",
  "window",
  "document",
  "localStorage",
  "sessionStorage",
  "module",
  "global"
  // Node.js
], Il = [].concat(
  Kn,
  Gn,
  Wn
);
function Zn(e) {
  const t = e.regex, n = (S, { after: v }) => {
    const U = "</" + S[0].slice(1);
    return S.input.indexOf(U, v) !== -1;
  }, s = dn, r = {
    begin: "<>",
    end: "</>"
  }, o = /<[A-Za-z0-9\\._:-]+\s*\/>/, l = {
    begin: /<[A-Za-z0-9\\._:-]+/,
    end: /\/[A-Za-z0-9\\._:-]+>|\/>/,
    /**
     * @param {RegExpMatchArray} match
     * @param {CallbackResponse} response
     */
    isTrulyOpeningTag: (S, v) => {
      const U = S[0].length + S.index, ne = S.input[U];
      if (
        // HTML should not include another raw `<` inside a tag
        // nested type?
        // `<Array<Array<number>>`, etc.
        ne === "<" || // the , gives away that this is not HTML
        // `<T, A extends keyof T, V>`
        ne === ","
      ) {
        v.ignoreMatch();
        return;
      }
      ne === ">" && (n(S, { after: U }) || v.ignoreMatch());
      let ue;
      const _e = S.input.substring(U);
      if (ue = _e.match(/^\s*=/)) {
        v.ignoreMatch();
        return;
      }
      if ((ue = _e.match(/^\s+extends\s+/)) && ue.index === 0) {
        v.ignoreMatch();
        return;
      }
    }
  }, i = {
    $pattern: dn,
    keyword: Ml,
    literal: Al,
    built_in: Il,
    "variable.language": Cl
  }, a = "[0-9](_?[0-9])*", u = `\\.(${a})`, c = "0|[1-9](_?[0-9])*|0[0-7]*[89][0-9]*", p = {
    className: "number",
    variants: [
      // DecimalLiteral
      { begin: `(\\b(${c})((${u})|\\.)?|(${u}))[eE][+-]?(${a})\\b` },
      { begin: `\\b(${c})\\b((${u})\\b|\\.)?|(${u})\\b` },
      // DecimalBigIntegerLiteral
      { begin: "\\b(0|[1-9](_?[0-9])*)n\\b" },
      // NonDecimalIntegerLiteral
      { begin: "\\b0[xX][0-9a-fA-F](_?[0-9a-fA-F])*n?\\b" },
      { begin: "\\b0[bB][0-1](_?[0-1])*n?\\b" },
      { begin: "\\b0[oO][0-7](_?[0-7])*n?\\b" },
      // LegacyOctalIntegerLiteral (does not include underscore separators)
      // https://tc39.es/ecma262/#sec-additional-syntax-numeric-literals
      { begin: "\\b0[0-7]+n?\\b" }
    ],
    relevance: 0
  }, d = {
    className: "subst",
    begin: "\\$\\{",
    end: "\\}",
    keywords: i,
    contains: []
    // defined later
  }, _ = {
    begin: ".?html`",
    end: "",
    starts: {
      end: "`",
      returnEnd: !1,
      contains: [
        e.BACKSLASH_ESCAPE,
        d
      ],
      subLanguage: "xml"
    }
  }, f = {
    begin: ".?css`",
    end: "",
    starts: {
      end: "`",
      returnEnd: !1,
      contains: [
        e.BACKSLASH_ESCAPE,
        d
      ],
      subLanguage: "css"
    }
  }, k = {
    begin: ".?gql`",
    end: "",
    starts: {
      end: "`",
      returnEnd: !1,
      contains: [
        e.BACKSLASH_ESCAPE,
        d
      ],
      subLanguage: "graphql"
    }
  }, $ = {
    className: "string",
    begin: "`",
    end: "`",
    contains: [
      e.BACKSLASH_ESCAPE,
      d
    ]
  }, P = {
    className: "comment",
    variants: [
      e.COMMENT(
        /\/\*\*(?!\/)/,
        "\\*/",
        {
          relevance: 0,
          contains: [
            {
              begin: "(?=@[A-Za-z]+)",
              relevance: 0,
              contains: [
                {
                  className: "doctag",
                  begin: "@[A-Za-z]+"
                },
                {
                  className: "type",
                  begin: "\\{",
                  end: "\\}",
                  excludeEnd: !0,
                  excludeBegin: !0,
                  relevance: 0
                },
                {
                  className: "variable",
                  begin: s + "(?=\\s*(-)|$)",
                  endsParent: !0,
                  relevance: 0
                },
                // eat spaces (not newlines) so we can find
                // types or variables
                {
                  begin: /(?=[^\n])\s/,
                  relevance: 0
                }
              ]
            }
          ]
        }
      ),
      e.C_BLOCK_COMMENT_MODE,
      e.C_LINE_COMMENT_MODE
    ]
  }, R = [
    e.APOS_STRING_MODE,
    e.QUOTE_STRING_MODE,
    _,
    f,
    k,
    $,
    // Skip numbers when they are part of a variable name
    { match: /\$\d+/ },
    p
    // This is intentional:
    // See https://github.com/highlightjs/highlight.js/issues/3288
    // hljs.REGEXP_MODE
  ];
  d.contains = R.concat({
    // we need to pair up {} inside our subst to prevent
    // it from ending too early by matching another }
    begin: /\{/,
    end: /\}/,
    keywords: i,
    contains: [
      "self"
    ].concat(R)
  });
  const B = [].concat(P, d.contains), F = B.concat([
    // eat recursive parens in sub expressions
    {
      begin: /(\s*)\(/,
      end: /\)/,
      keywords: i,
      contains: ["self"].concat(B)
    }
  ]), M = {
    className: "params",
    // convert this to negative lookbehind in v12
    begin: /(\s*)\(/,
    // to match the parms with
    end: /\)/,
    excludeBegin: !0,
    excludeEnd: !0,
    keywords: i,
    contains: F
  }, q = {
    variants: [
      // class Car extends vehicle
      {
        match: [
          /class/,
          /\s+/,
          s,
          /\s+/,
          /extends/,
          /\s+/,
          t.concat(s, "(", t.concat(/\./, s), ")*")
        ],
        scope: {
          1: "keyword",
          3: "title.class",
          5: "keyword",
          7: "title.class.inherited"
        }
      },
      // class Car
      {
        match: [
          /class/,
          /\s+/,
          s
        ],
        scope: {
          1: "keyword",
          3: "title.class"
        }
      }
    ]
  }, T = {
    relevance: 0,
    match: t.either(
      // Hard coded exceptions
      /\bJSON/,
      // Float32Array, OutT
      /\b[A-Z][a-z]+([A-Z][a-z]*|\d)*/,
      // CSSFactory, CSSFactoryT
      /\b[A-Z]{2,}([A-Z][a-z]+|\d)+([A-Z][a-z]*)*/,
      // FPs, FPsT
      /\b[A-Z]{2,}[a-z]+([A-Z][a-z]+|\d)*([A-Z][a-z]*)*/
      // P
      // single letters are not highlighted
      // BLAH
      // this will be flagged as a UPPER_CASE_CONSTANT instead
    ),
    className: "title.class",
    keywords: {
      _: [
        // se we still get relevance credit for JS library classes
        ...Gn,
        ...Wn
      ]
    }
  }, te = {
    label: "use_strict",
    className: "meta",
    relevance: 10,
    begin: /^\s*['"]use (strict|asm)['"]/
  }, re = {
    variants: [
      {
        match: [
          /function/,
          /\s+/,
          s,
          /(?=\s*\()/
        ]
      },
      // anonymous function
      {
        match: [
          /function/,
          /\s*(?=\()/
        ]
      }
    ],
    className: {
      1: "keyword",
      3: "title.function"
    },
    label: "func.def",
    contains: [M],
    illegal: /%/
  }, de = {
    relevance: 0,
    match: /\b[A-Z][A-Z_0-9]+\b/,
    className: "variable.constant"
  };
  function se(S) {
    return t.concat("(?!", S.join("|"), ")");
  }
  const E = {
    match: t.concat(
      /\b/,
      se([
        ...Kn,
        "super",
        "import"
      ].map((S) => `${S}\\s*\\(`)),
      s,
      t.lookahead(/\s*\(/)
    ),
    className: "title.function",
    relevance: 0
  }, C = {
    begin: t.concat(/\./, t.lookahead(
      t.concat(s, /(?![0-9A-Za-z$_(])/)
    )),
    end: s,
    excludeBegin: !0,
    keywords: "prototype",
    className: "property",
    relevance: 0
  }, O = {
    match: [
      /get|set/,
      /\s+/,
      s,
      /(?=\()/
    ],
    className: {
      1: "keyword",
      3: "title.function"
    },
    contains: [
      {
        // eat to avoid empty params
        begin: /\(\)/
      },
      M
    ]
  }, y = "(\\([^()]*(\\([^()]*(\\([^()]*\\)[^()]*)*\\)[^()]*)*\\)|" + e.UNDERSCORE_IDENT_RE + ")\\s*=>", m = {
    match: [
      /const|var|let/,
      /\s+/,
      s,
      /\s*/,
      /=\s*/,
      /(async\s*)?/,
      // async is optional
      t.lookahead(y)
    ],
    keywords: "async",
    className: {
      1: "keyword",
      3: "title.function"
    },
    contains: [
      M
    ]
  };
  return {
    name: "JavaScript",
    aliases: ["js", "jsx", "mjs", "cjs"],
    keywords: i,
    // this will be extended by TypeScript
    exports: { PARAMS_CONTAINS: F, CLASS_REFERENCE: T },
    illegal: /#(?![$_A-z])/,
    contains: [
      e.SHEBANG({
        label: "shebang",
        binary: "node",
        relevance: 5
      }),
      te,
      e.APOS_STRING_MODE,
      e.QUOTE_STRING_MODE,
      _,
      f,
      k,
      $,
      P,
      // Skip numbers when they are part of a variable name
      { match: /\$\d+/ },
      p,
      T,
      {
        scope: "attr",
        match: s + t.lookahead(":"),
        relevance: 0
      },
      m,
      {
        // "value" container
        begin: "(" + e.RE_STARTERS_RE + "|\\b(case|return|throw)\\b)\\s*",
        keywords: "return throw case",
        relevance: 0,
        contains: [
          P,
          e.REGEXP_MODE,
          {
            className: "function",
            // we have to count the parens to make sure we actually have the
            // correct bounding ( ) before the =>.  There could be any number of
            // sub-expressions inside also surrounded by parens.
            begin: y,
            returnBegin: !0,
            end: "\\s*=>",
            contains: [
              {
                className: "params",
                variants: [
                  {
                    begin: e.UNDERSCORE_IDENT_RE,
                    relevance: 0
                  },
                  {
                    className: null,
                    begin: /\(\s*\)/,
                    skip: !0
                  },
                  {
                    begin: /(\s*)\(/,
                    end: /\)/,
                    excludeBegin: !0,
                    excludeEnd: !0,
                    keywords: i,
                    contains: F
                  }
                ]
              }
            ]
          },
          {
            // could be a comma delimited list of params to a function call
            begin: /,/,
            relevance: 0
          },
          {
            match: /\s+/,
            relevance: 0
          },
          {
            // JSX
            variants: [
              { begin: r.begin, end: r.end },
              { match: o },
              {
                begin: l.begin,
                // we carefully check the opening tag to see if it truly
                // is a tag and not a false positive
                "on:begin": l.isTrulyOpeningTag,
                end: l.end
              }
            ],
            subLanguage: "xml",
            contains: [
              {
                begin: l.begin,
                end: l.end,
                skip: !0,
                contains: ["self"]
              }
            ]
          }
        ]
      },
      re,
      {
        // prevent this from getting swallowed up by function
        // since they appear "function like"
        beginKeywords: "while if switch catch for"
      },
      {
        // we have to count the parens to make sure we actually have the correct
        // bounding ( ).  There could be any number of sub-expressions inside
        // also surrounded by parens.
        begin: "\\b(?!function)" + e.UNDERSCORE_IDENT_RE + "\\([^()]*(\\([^()]*(\\([^()]*\\)[^()]*)*\\)[^()]*)*\\)\\s*\\{",
        // end parens
        returnBegin: !0,
        label: "func.def",
        contains: [
          M,
          e.inherit(e.TITLE_MODE, { begin: s, className: "title.function" })
        ]
      },
      // catch ... so it won't trigger the property rule below
      {
        match: /\.\.\./,
        relevance: 0
      },
      C,
      // hack: prevents detection of keywords in some circumstances
      // .keyword()
      // $keyword = x
      {
        match: "\\$" + s,
        relevance: 0
      },
      {
        match: [/\bconstructor(?=\s*\()/],
        className: { 1: "title.function" },
        contains: [M]
      },
      E,
      de,
      q,
      O,
      {
        match: /\$[(.]/
        // relevance booster for a pattern common to JS libs: `$(something)` and `$.something`
      }
    ]
  };
}
const rt = "[A-Za-z$_][0-9A-Za-z$_]*", Xn = [
  "as",
  // for exports
  "in",
  "of",
  "if",
  "for",
  "while",
  "finally",
  "var",
  "new",
  "function",
  "do",
  "return",
  "void",
  "else",
  "break",
  "catch",
  "instanceof",
  "with",
  "throw",
  "case",
  "default",
  "try",
  "switch",
  "continue",
  "typeof",
  "delete",
  "let",
  "yield",
  "const",
  "class",
  // JS handles these with a special rule
  // "get",
  // "set",
  "debugger",
  "async",
  "await",
  "static",
  "import",
  "from",
  "export",
  "extends",
  // It's reached stage 3, which is "recommended for implementation":
  "using"
], Yn = [
  "true",
  "false",
  "null",
  "undefined",
  "NaN",
  "Infinity"
], Jn = [
  // Fundamental objects
  "Object",
  "Function",
  "Boolean",
  "Symbol",
  // numbers and dates
  "Math",
  "Date",
  "Number",
  "BigInt",
  // text
  "String",
  "RegExp",
  // Indexed collections
  "Array",
  "Float32Array",
  "Float64Array",
  "Int8Array",
  "Uint8Array",
  "Uint8ClampedArray",
  "Int16Array",
  "Int32Array",
  "Uint16Array",
  "Uint32Array",
  "BigInt64Array",
  "BigUint64Array",
  // Keyed collections
  "Set",
  "Map",
  "WeakSet",
  "WeakMap",
  // Structured data
  "ArrayBuffer",
  "SharedArrayBuffer",
  "Atomics",
  "DataView",
  "JSON",
  // Control abstraction objects
  "Promise",
  "Generator",
  "GeneratorFunction",
  "AsyncFunction",
  // Reflection
  "Reflect",
  "Proxy",
  // Internationalization
  "Intl",
  // WebAssembly
  "WebAssembly"
], Qn = [
  "Error",
  "EvalError",
  "InternalError",
  "RangeError",
  "ReferenceError",
  "SyntaxError",
  "TypeError",
  "URIError"
], es = [
  "setInterval",
  "setTimeout",
  "clearInterval",
  "clearTimeout",
  "require",
  "exports",
  "eval",
  "isFinite",
  "isNaN",
  "parseFloat",
  "parseInt",
  "decodeURI",
  "decodeURIComponent",
  "encodeURI",
  "encodeURIComponent",
  "escape",
  "unescape"
], ts = [
  "arguments",
  "this",
  "super",
  "console",
  "window",
  "document",
  "localStorage",
  "sessionStorage",
  "module",
  "global"
  // Node.js
], ns = [].concat(
  es,
  Jn,
  Qn
);
function Ol(e) {
  const t = e.regex, n = (S, { after: v }) => {
    const U = "</" + S[0].slice(1);
    return S.input.indexOf(U, v) !== -1;
  }, s = rt, r = {
    begin: "<>",
    end: "</>"
  }, o = /<[A-Za-z0-9\\._:-]+\s*\/>/, l = {
    begin: /<[A-Za-z0-9\\._:-]+/,
    end: /\/[A-Za-z0-9\\._:-]+>|\/>/,
    /**
     * @param {RegExpMatchArray} match
     * @param {CallbackResponse} response
     */
    isTrulyOpeningTag: (S, v) => {
      const U = S[0].length + S.index, ne = S.input[U];
      if (
        // HTML should not include another raw `<` inside a tag
        // nested type?
        // `<Array<Array<number>>`, etc.
        ne === "<" || // the , gives away that this is not HTML
        // `<T, A extends keyof T, V>`
        ne === ","
      ) {
        v.ignoreMatch();
        return;
      }
      ne === ">" && (n(S, { after: U }) || v.ignoreMatch());
      let ue;
      const _e = S.input.substring(U);
      if (ue = _e.match(/^\s*=/)) {
        v.ignoreMatch();
        return;
      }
      if ((ue = _e.match(/^\s+extends\s+/)) && ue.index === 0) {
        v.ignoreMatch();
        return;
      }
    }
  }, i = {
    $pattern: rt,
    keyword: Xn,
    literal: Yn,
    built_in: ns,
    "variable.language": ts
  }, a = "[0-9](_?[0-9])*", u = `\\.(${a})`, c = "0|[1-9](_?[0-9])*|0[0-7]*[89][0-9]*", p = {
    className: "number",
    variants: [
      // DecimalLiteral
      { begin: `(\\b(${c})((${u})|\\.)?|(${u}))[eE][+-]?(${a})\\b` },
      { begin: `\\b(${c})\\b((${u})\\b|\\.)?|(${u})\\b` },
      // DecimalBigIntegerLiteral
      { begin: "\\b(0|[1-9](_?[0-9])*)n\\b" },
      // NonDecimalIntegerLiteral
      { begin: "\\b0[xX][0-9a-fA-F](_?[0-9a-fA-F])*n?\\b" },
      { begin: "\\b0[bB][0-1](_?[0-1])*n?\\b" },
      { begin: "\\b0[oO][0-7](_?[0-7])*n?\\b" },
      // LegacyOctalIntegerLiteral (does not include underscore separators)
      // https://tc39.es/ecma262/#sec-additional-syntax-numeric-literals
      { begin: "\\b0[0-7]+n?\\b" }
    ],
    relevance: 0
  }, d = {
    className: "subst",
    begin: "\\$\\{",
    end: "\\}",
    keywords: i,
    contains: []
    // defined later
  }, _ = {
    begin: ".?html`",
    end: "",
    starts: {
      end: "`",
      returnEnd: !1,
      contains: [
        e.BACKSLASH_ESCAPE,
        d
      ],
      subLanguage: "xml"
    }
  }, f = {
    begin: ".?css`",
    end: "",
    starts: {
      end: "`",
      returnEnd: !1,
      contains: [
        e.BACKSLASH_ESCAPE,
        d
      ],
      subLanguage: "css"
    }
  }, k = {
    begin: ".?gql`",
    end: "",
    starts: {
      end: "`",
      returnEnd: !1,
      contains: [
        e.BACKSLASH_ESCAPE,
        d
      ],
      subLanguage: "graphql"
    }
  }, $ = {
    className: "string",
    begin: "`",
    end: "`",
    contains: [
      e.BACKSLASH_ESCAPE,
      d
    ]
  }, P = {
    className: "comment",
    variants: [
      e.COMMENT(
        /\/\*\*(?!\/)/,
        "\\*/",
        {
          relevance: 0,
          contains: [
            {
              begin: "(?=@[A-Za-z]+)",
              relevance: 0,
              contains: [
                {
                  className: "doctag",
                  begin: "@[A-Za-z]+"
                },
                {
                  className: "type",
                  begin: "\\{",
                  end: "\\}",
                  excludeEnd: !0,
                  excludeBegin: !0,
                  relevance: 0
                },
                {
                  className: "variable",
                  begin: s + "(?=\\s*(-)|$)",
                  endsParent: !0,
                  relevance: 0
                },
                // eat spaces (not newlines) so we can find
                // types or variables
                {
                  begin: /(?=[^\n])\s/,
                  relevance: 0
                }
              ]
            }
          ]
        }
      ),
      e.C_BLOCK_COMMENT_MODE,
      e.C_LINE_COMMENT_MODE
    ]
  }, R = [
    e.APOS_STRING_MODE,
    e.QUOTE_STRING_MODE,
    _,
    f,
    k,
    $,
    // Skip numbers when they are part of a variable name
    { match: /\$\d+/ },
    p
    // This is intentional:
    // See https://github.com/highlightjs/highlight.js/issues/3288
    // hljs.REGEXP_MODE
  ];
  d.contains = R.concat({
    // we need to pair up {} inside our subst to prevent
    // it from ending too early by matching another }
    begin: /\{/,
    end: /\}/,
    keywords: i,
    contains: [
      "self"
    ].concat(R)
  });
  const B = [].concat(P, d.contains), F = B.concat([
    // eat recursive parens in sub expressions
    {
      begin: /(\s*)\(/,
      end: /\)/,
      keywords: i,
      contains: ["self"].concat(B)
    }
  ]), M = {
    className: "params",
    // convert this to negative lookbehind in v12
    begin: /(\s*)\(/,
    // to match the parms with
    end: /\)/,
    excludeBegin: !0,
    excludeEnd: !0,
    keywords: i,
    contains: F
  }, q = {
    variants: [
      // class Car extends vehicle
      {
        match: [
          /class/,
          /\s+/,
          s,
          /\s+/,
          /extends/,
          /\s+/,
          t.concat(s, "(", t.concat(/\./, s), ")*")
        ],
        scope: {
          1: "keyword",
          3: "title.class",
          5: "keyword",
          7: "title.class.inherited"
        }
      },
      // class Car
      {
        match: [
          /class/,
          /\s+/,
          s
        ],
        scope: {
          1: "keyword",
          3: "title.class"
        }
      }
    ]
  }, T = {
    relevance: 0,
    match: t.either(
      // Hard coded exceptions
      /\bJSON/,
      // Float32Array, OutT
      /\b[A-Z][a-z]+([A-Z][a-z]*|\d)*/,
      // CSSFactory, CSSFactoryT
      /\b[A-Z]{2,}([A-Z][a-z]+|\d)+([A-Z][a-z]*)*/,
      // FPs, FPsT
      /\b[A-Z]{2,}[a-z]+([A-Z][a-z]+|\d)*([A-Z][a-z]*)*/
      // P
      // single letters are not highlighted
      // BLAH
      // this will be flagged as a UPPER_CASE_CONSTANT instead
    ),
    className: "title.class",
    keywords: {
      _: [
        // se we still get relevance credit for JS library classes
        ...Jn,
        ...Qn
      ]
    }
  }, te = {
    label: "use_strict",
    className: "meta",
    relevance: 10,
    begin: /^\s*['"]use (strict|asm)['"]/
  }, re = {
    variants: [
      {
        match: [
          /function/,
          /\s+/,
          s,
          /(?=\s*\()/
        ]
      },
      // anonymous function
      {
        match: [
          /function/,
          /\s*(?=\()/
        ]
      }
    ],
    className: {
      1: "keyword",
      3: "title.function"
    },
    label: "func.def",
    contains: [M],
    illegal: /%/
  }, de = {
    relevance: 0,
    match: /\b[A-Z][A-Z_0-9]+\b/,
    className: "variable.constant"
  };
  function se(S) {
    return t.concat("(?!", S.join("|"), ")");
  }
  const E = {
    match: t.concat(
      /\b/,
      se([
        ...es,
        "super",
        "import"
      ].map((S) => `${S}\\s*\\(`)),
      s,
      t.lookahead(/\s*\(/)
    ),
    className: "title.function",
    relevance: 0
  }, C = {
    begin: t.concat(/\./, t.lookahead(
      t.concat(s, /(?![0-9A-Za-z$_(])/)
    )),
    end: s,
    excludeBegin: !0,
    keywords: "prototype",
    className: "property",
    relevance: 0
  }, O = {
    match: [
      /get|set/,
      /\s+/,
      s,
      /(?=\()/
    ],
    className: {
      1: "keyword",
      3: "title.function"
    },
    contains: [
      {
        // eat to avoid empty params
        begin: /\(\)/
      },
      M
    ]
  }, y = "(\\([^()]*(\\([^()]*(\\([^()]*\\)[^()]*)*\\)[^()]*)*\\)|" + e.UNDERSCORE_IDENT_RE + ")\\s*=>", m = {
    match: [
      /const|var|let/,
      /\s+/,
      s,
      /\s*/,
      /=\s*/,
      /(async\s*)?/,
      // async is optional
      t.lookahead(y)
    ],
    keywords: "async",
    className: {
      1: "keyword",
      3: "title.function"
    },
    contains: [
      M
    ]
  };
  return {
    name: "JavaScript",
    aliases: ["js", "jsx", "mjs", "cjs"],
    keywords: i,
    // this will be extended by TypeScript
    exports: { PARAMS_CONTAINS: F, CLASS_REFERENCE: T },
    illegal: /#(?![$_A-z])/,
    contains: [
      e.SHEBANG({
        label: "shebang",
        binary: "node",
        relevance: 5
      }),
      te,
      e.APOS_STRING_MODE,
      e.QUOTE_STRING_MODE,
      _,
      f,
      k,
      $,
      P,
      // Skip numbers when they are part of a variable name
      { match: /\$\d+/ },
      p,
      T,
      {
        scope: "attr",
        match: s + t.lookahead(":"),
        relevance: 0
      },
      m,
      {
        // "value" container
        begin: "(" + e.RE_STARTERS_RE + "|\\b(case|return|throw)\\b)\\s*",
        keywords: "return throw case",
        relevance: 0,
        contains: [
          P,
          e.REGEXP_MODE,
          {
            className: "function",
            // we have to count the parens to make sure we actually have the
            // correct bounding ( ) before the =>.  There could be any number of
            // sub-expressions inside also surrounded by parens.
            begin: y,
            returnBegin: !0,
            end: "\\s*=>",
            contains: [
              {
                className: "params",
                variants: [
                  {
                    begin: e.UNDERSCORE_IDENT_RE,
                    relevance: 0
                  },
                  {
                    className: null,
                    begin: /\(\s*\)/,
                    skip: !0
                  },
                  {
                    begin: /(\s*)\(/,
                    end: /\)/,
                    excludeBegin: !0,
                    excludeEnd: !0,
                    keywords: i,
                    contains: F
                  }
                ]
              }
            ]
          },
          {
            // could be a comma delimited list of params to a function call
            begin: /,/,
            relevance: 0
          },
          {
            match: /\s+/,
            relevance: 0
          },
          {
            // JSX
            variants: [
              { begin: r.begin, end: r.end },
              { match: o },
              {
                begin: l.begin,
                // we carefully check the opening tag to see if it truly
                // is a tag and not a false positive
                "on:begin": l.isTrulyOpeningTag,
                end: l.end
              }
            ],
            subLanguage: "xml",
            contains: [
              {
                begin: l.begin,
                end: l.end,
                skip: !0,
                contains: ["self"]
              }
            ]
          }
        ]
      },
      re,
      {
        // prevent this from getting swallowed up by function
        // since they appear "function like"
        beginKeywords: "while if switch catch for"
      },
      {
        // we have to count the parens to make sure we actually have the correct
        // bounding ( ).  There could be any number of sub-expressions inside
        // also surrounded by parens.
        begin: "\\b(?!function)" + e.UNDERSCORE_IDENT_RE + "\\([^()]*(\\([^()]*(\\([^()]*\\)[^()]*)*\\)[^()]*)*\\)\\s*\\{",
        // end parens
        returnBegin: !0,
        label: "func.def",
        contains: [
          M,
          e.inherit(e.TITLE_MODE, { begin: s, className: "title.function" })
        ]
      },
      // catch ... so it won't trigger the property rule below
      {
        match: /\.\.\./,
        relevance: 0
      },
      C,
      // hack: prevents detection of keywords in some circumstances
      // .keyword()
      // $keyword = x
      {
        match: "\\$" + s,
        relevance: 0
      },
      {
        match: [/\bconstructor(?=\s*\()/],
        className: { 1: "title.function" },
        contains: [M]
      },
      E,
      de,
      q,
      O,
      {
        match: /\$[(.]/
        // relevance booster for a pattern common to JS libs: `$(something)` and `$.something`
      }
    ]
  };
}
function ss(e) {
  const t = e.regex, n = Ol(e), s = rt, r = [
    "any",
    "void",
    "number",
    "boolean",
    "string",
    "object",
    "never",
    "symbol",
    "bigint",
    "unknown"
  ], o = {
    begin: [
      /namespace/,
      /\s+/,
      e.IDENT_RE
    ],
    beginScope: {
      1: "keyword",
      3: "title.class"
    }
  }, l = {
    beginKeywords: "interface",
    end: /\{/,
    excludeEnd: !0,
    keywords: {
      keyword: "interface extends",
      built_in: r
    },
    contains: [n.exports.CLASS_REFERENCE]
  }, i = {
    className: "meta",
    relevance: 10,
    begin: /^\s*['"]use strict['"]/
  }, a = [
    "type",
    // "namespace",
    "interface",
    "public",
    "private",
    "protected",
    "implements",
    "declare",
    "abstract",
    "readonly",
    "enum",
    "override",
    "satisfies"
  ], u = {
    $pattern: rt,
    keyword: Xn.concat(a),
    literal: Yn,
    built_in: ns.concat(r),
    "variable.language": ts
  }, c = {
    className: "meta",
    begin: "@" + s
  }, p = (k, $, I) => {
    const P = k.contains.findIndex((R) => R.label === $);
    if (P === -1)
      throw new Error("can not find mode to replace");
    k.contains.splice(P, 1, I);
  };
  Object.assign(n.keywords, u), n.exports.PARAMS_CONTAINS.push(c);
  const d = n.contains.find((k) => k.scope === "attr"), _ = Object.assign(
    {},
    d,
    { match: t.concat(s, t.lookahead(/\s*\?:/)) }
  );
  n.exports.PARAMS_CONTAINS.push([
    n.exports.CLASS_REFERENCE,
    // class reference for highlighting the params types
    d,
    // highlight the params key
    _
    // Added for optional property assignment highlighting
  ]), n.contains = n.contains.concat([
    c,
    o,
    l,
    _
    // Added for optional property assignment highlighting
  ]), p(n, "shebang", e.SHEBANG()), p(n, "use_strict", i);
  const f = n.contains.find((k) => k.label === "func.def");
  return f.relevance = 0, Object.assign(n, {
    name: "TypeScript",
    aliases: [
      "ts",
      "tsx",
      "mts",
      "cts"
    ]
  }), n;
}
function rs(e) {
  const t = [
    "bool",
    "byte",
    "char",
    "decimal",
    "delegate",
    "double",
    "dynamic",
    "enum",
    "float",
    "int",
    "long",
    "nint",
    "nuint",
    "object",
    "sbyte",
    "short",
    "string",
    "ulong",
    "uint",
    "ushort"
  ], n = [
    "public",
    "private",
    "protected",
    "static",
    "internal",
    "protected",
    "abstract",
    "async",
    "extern",
    "override",
    "unsafe",
    "virtual",
    "new",
    "sealed",
    "partial"
  ], s = [
    "default",
    "false",
    "null",
    "true"
  ], r = [
    "abstract",
    "as",
    "base",
    "break",
    "case",
    "catch",
    "class",
    "const",
    "continue",
    "do",
    "else",
    "event",
    "explicit",
    "extern",
    "finally",
    "fixed",
    "for",
    "foreach",
    "goto",
    "if",
    "implicit",
    "in",
    "interface",
    "internal",
    "is",
    "lock",
    "namespace",
    "new",
    "operator",
    "out",
    "override",
    "params",
    "private",
    "protected",
    "public",
    "readonly",
    "record",
    "ref",
    "return",
    "scoped",
    "sealed",
    "sizeof",
    "stackalloc",
    "static",
    "struct",
    "switch",
    "this",
    "throw",
    "try",
    "typeof",
    "unchecked",
    "unsafe",
    "using",
    "virtual",
    "void",
    "volatile",
    "while"
  ], o = [
    "add",
    "alias",
    "and",
    "ascending",
    "args",
    "async",
    "await",
    "by",
    "descending",
    "dynamic",
    "equals",
    "file",
    "from",
    "get",
    "global",
    "group",
    "init",
    "into",
    "join",
    "let",
    "nameof",
    "not",
    "notnull",
    "on",
    "or",
    "orderby",
    "partial",
    "record",
    "remove",
    "required",
    "scoped",
    "select",
    "set",
    "unmanaged",
    "value|0",
    "var",
    "when",
    "where",
    "with",
    "yield"
  ], l = {
    keyword: r.concat(o),
    built_in: t,
    literal: s
  }, i = e.inherit(e.TITLE_MODE, { begin: "[a-zA-Z](\\.?\\w)*" }), a = {
    className: "number",
    variants: [
      { begin: "\\b(0b[01']+)" },
      { begin: "(-?)\\b([\\d']+(\\.[\\d']*)?|\\.[\\d']+)(u|U|l|L|ul|UL|f|F|b|B)" },
      { begin: "(-?)(\\b0[xX][a-fA-F0-9']+|(\\b[\\d']+(\\.[\\d']*)?|\\.[\\d']+)([eE][-+]?[\\d']+)?)" }
    ],
    relevance: 0
  }, u = {
    className: "string",
    begin: /"""("*)(?!")(.|\n)*?"""\1/,
    relevance: 1
  }, c = {
    className: "string",
    begin: '@"',
    end: '"',
    contains: [{ begin: '""' }]
  }, p = e.inherit(c, { illegal: /\n/ }), d = {
    className: "subst",
    begin: /\{/,
    end: /\}/,
    keywords: l
  }, _ = e.inherit(d, { illegal: /\n/ }), f = {
    className: "string",
    begin: /\$"/,
    end: '"',
    illegal: /\n/,
    contains: [
      { begin: /\{\{/ },
      { begin: /\}\}/ },
      e.BACKSLASH_ESCAPE,
      _
    ]
  }, k = {
    className: "string",
    begin: /\$@"/,
    end: '"',
    contains: [
      { begin: /\{\{/ },
      { begin: /\}\}/ },
      { begin: '""' },
      d
    ]
  }, $ = e.inherit(k, {
    illegal: /\n/,
    contains: [
      { begin: /\{\{/ },
      { begin: /\}\}/ },
      { begin: '""' },
      _
    ]
  });
  d.contains = [
    k,
    f,
    c,
    e.APOS_STRING_MODE,
    e.QUOTE_STRING_MODE,
    a,
    e.C_BLOCK_COMMENT_MODE
  ], _.contains = [
    $,
    f,
    p,
    e.APOS_STRING_MODE,
    e.QUOTE_STRING_MODE,
    a,
    e.inherit(e.C_BLOCK_COMMENT_MODE, { illegal: /\n/ })
  ];
  const I = { variants: [
    u,
    k,
    f,
    c,
    e.APOS_STRING_MODE,
    e.QUOTE_STRING_MODE
  ] }, P = {
    begin: "<",
    end: ">",
    contains: [
      { beginKeywords: "in out" },
      i
    ]
  }, R = e.IDENT_RE + "(<" + e.IDENT_RE + "(\\s*,\\s*" + e.IDENT_RE + ")*>)?(\\[\\])?", B = {
    // prevents expressions like `@class` from incorrect flagging
    // `class` as a keyword
    begin: "@" + e.IDENT_RE,
    relevance: 0
  };
  return {
    name: "C#",
    aliases: [
      "cs",
      "c#"
    ],
    keywords: l,
    illegal: /::/,
    contains: [
      e.COMMENT(
        "///",
        "$",
        {
          returnBegin: !0,
          contains: [
            {
              className: "doctag",
              variants: [
                {
                  begin: "///",
                  relevance: 0
                },
                { begin: "<!--|-->" },
                {
                  begin: "</?",
                  end: ">"
                }
              ]
            }
          ]
        }
      ),
      e.C_LINE_COMMENT_MODE,
      e.C_BLOCK_COMMENT_MODE,
      {
        className: "meta",
        begin: "#",
        end: "$",
        keywords: { keyword: "if else elif endif define undef warning error line region endregion pragma checksum" }
      },
      I,
      a,
      {
        beginKeywords: "class interface",
        relevance: 0,
        end: /[{;=]/,
        illegal: /[^\s:,]/,
        contains: [
          { beginKeywords: "where class" },
          i,
          P,
          e.C_LINE_COMMENT_MODE,
          e.C_BLOCK_COMMENT_MODE
        ]
      },
      {
        beginKeywords: "namespace",
        relevance: 0,
        end: /[{;=]/,
        illegal: /[^\s:]/,
        contains: [
          i,
          e.C_LINE_COMMENT_MODE,
          e.C_BLOCK_COMMENT_MODE
        ]
      },
      {
        beginKeywords: "record",
        relevance: 0,
        end: /[{;=]/,
        illegal: /[^\s:]/,
        contains: [
          i,
          P,
          e.C_LINE_COMMENT_MODE,
          e.C_BLOCK_COMMENT_MODE
        ]
      },
      {
        // [Attributes("")]
        className: "meta",
        begin: "^\\s*\\[(?=[\\w])",
        excludeBegin: !0,
        end: "\\]",
        excludeEnd: !0,
        contains: [
          {
            className: "string",
            begin: /"/,
            end: /"/
          }
        ]
      },
      {
        // Expression keywords prevent 'keyword Name(...)' from being
        // recognized as a function definition
        beginKeywords: "new return throw await else",
        relevance: 0
      },
      {
        className: "function",
        begin: "(" + R + "\\s+)+" + e.IDENT_RE + "\\s*(<[^=]+>\\s*)?\\(",
        returnBegin: !0,
        end: /\s*[{;=]/,
        excludeEnd: !0,
        keywords: l,
        contains: [
          // prevents these from being highlighted `title`
          {
            beginKeywords: n.join(" "),
            relevance: 0
          },
          {
            begin: e.IDENT_RE + "\\s*(<[^=]+>\\s*)?\\(",
            returnBegin: !0,
            contains: [
              e.TITLE_MODE,
              P
            ],
            relevance: 0
          },
          { match: /\(\)/ },
          {
            className: "params",
            begin: /\(/,
            end: /\)/,
            excludeBegin: !0,
            excludeEnd: !0,
            keywords: l,
            relevance: 0,
            contains: [
              I,
              a,
              e.C_BLOCK_COMMENT_MODE
            ]
          },
          e.C_LINE_COMMENT_MODE,
          e.C_BLOCK_COMMENT_MODE
        ]
      },
      B
    ]
  };
}
function Lt(e) {
  const t = e.regex, n = t.concat(/[\p{L}_]/u, t.optional(/[\p{L}0-9_.-]*:/u), /[\p{L}0-9_.-]*/u), s = /[\p{L}0-9._:-]+/u, r = {
    className: "symbol",
    begin: /&[a-z]+;|&#[0-9]+;|&#x[a-f0-9]+;/
  }, o = {
    begin: /\s/,
    contains: [
      {
        className: "keyword",
        begin: /#?[a-z_][a-z1-9_-]+/,
        illegal: /\n/
      }
    ]
  }, l = e.inherit(o, {
    begin: /\(/,
    end: /\)/
  }), i = e.inherit(e.APOS_STRING_MODE, { className: "string" }), a = e.inherit(e.QUOTE_STRING_MODE, { className: "string" }), u = {
    endsWithParent: !0,
    illegal: /</,
    relevance: 0,
    contains: [
      {
        className: "attr",
        begin: s,
        relevance: 0
      },
      {
        begin: /=\s*/,
        relevance: 0,
        contains: [
          {
            className: "string",
            endsParent: !0,
            variants: [
              {
                begin: /"/,
                end: /"/,
                contains: [r]
              },
              {
                begin: /'/,
                end: /'/,
                contains: [r]
              },
              { begin: /[^\s"'=<>`]+/ }
            ]
          }
        ]
      }
    ]
  };
  return {
    name: "HTML, XML",
    aliases: [
      "html",
      "xhtml",
      "rss",
      "atom",
      "xjb",
      "xsd",
      "xsl",
      "plist",
      "wsf",
      "svg"
    ],
    case_insensitive: !0,
    unicodeRegex: !0,
    contains: [
      {
        className: "meta",
        begin: /<![a-z]/,
        end: />/,
        relevance: 10,
        contains: [
          o,
          a,
          i,
          l,
          {
            begin: /\[/,
            end: /\]/,
            contains: [
              {
                className: "meta",
                begin: /<![a-z]/,
                end: />/,
                contains: [
                  o,
                  l,
                  a,
                  i
                ]
              }
            ]
          }
        ]
      },
      e.COMMENT(
        /<!--/,
        /-->/,
        { relevance: 10 }
      ),
      {
        begin: /<!\[CDATA\[/,
        end: /\]\]>/,
        relevance: 10
      },
      r,
      // xml processing instructions
      {
        className: "meta",
        end: /\?>/,
        variants: [
          {
            begin: /<\?xml/,
            relevance: 10,
            contains: [
              a
            ]
          },
          {
            begin: /<\?[a-z][a-z0-9]+/
          }
        ]
      },
      {
        className: "tag",
        /*
        The lookahead pattern (?=...) ensures that 'begin' only matches
        '<style' as a single word, followed by a whitespace or an
        ending bracket.
        */
        begin: /<style(?=\s|>)/,
        end: />/,
        keywords: { name: "style" },
        contains: [u],
        starts: {
          end: /<\/style>/,
          returnEnd: !0,
          subLanguage: [
            "css",
            "xml"
          ]
        }
      },
      {
        className: "tag",
        // See the comment in the <style tag about the lookahead pattern
        begin: /<script(?=\s|>)/,
        end: />/,
        keywords: { name: "script" },
        contains: [u],
        starts: {
          end: /<\/script>/,
          returnEnd: !0,
          subLanguage: [
            "javascript",
            "handlebars",
            "xml"
          ]
        }
      },
      // we need this for now for jSX
      {
        className: "tag",
        begin: /<>|<\/>/
      },
      // open tag
      {
        className: "tag",
        begin: t.concat(
          /</,
          t.lookahead(t.concat(
            n,
            // <tag/>
            // <tag>
            // <tag ...
            t.either(/\/>/, />/, /\s/)
          ))
        ),
        end: /\/?>/,
        contains: [
          {
            className: "name",
            begin: n,
            relevance: 0,
            starts: u
          }
        ]
      },
      // close tag
      {
        className: "tag",
        begin: t.concat(
          /<\//,
          t.lookahead(t.concat(
            n,
            />/
          ))
        ),
        contains: [
          {
            className: "name",
            begin: n,
            relevance: 0
          },
          {
            begin: />/,
            relevance: 0,
            endsParent: !0
          }
        ]
      }
    ]
  };
}
const Nl = (e) => ({
  IMPORTANT: {
    scope: "meta",
    begin: "!important"
  },
  BLOCK_COMMENT: e.C_BLOCK_COMMENT_MODE,
  HEXCOLOR: {
    scope: "number",
    begin: /#(([0-9a-fA-F]{3,4})|(([0-9a-fA-F]{2}){3,4}))\b/
  },
  FUNCTION_DISPATCH: {
    className: "built_in",
    begin: /[\w-]+(?=\()/
  },
  ATTRIBUTE_SELECTOR_MODE: {
    scope: "selector-attr",
    begin: /\[/,
    end: /\]/,
    illegal: "$",
    contains: [
      e.APOS_STRING_MODE,
      e.QUOTE_STRING_MODE
    ]
  },
  CSS_NUMBER_MODE: {
    scope: "number",
    begin: e.NUMBER_RE + "(%|em|ex|ch|rem|vw|vh|vmin|vmax|cm|mm|in|pt|pc|px|deg|grad|rad|turn|s|ms|Hz|kHz|dpi|dpcm|dppx)?",
    relevance: 0
  },
  CSS_VARIABLE: {
    className: "attr",
    begin: /--[A-Za-z_][A-Za-z0-9_-]*/
  }
}), Rl = [
  "a",
  "abbr",
  "address",
  "article",
  "aside",
  "audio",
  "b",
  "blockquote",
  "body",
  "button",
  "canvas",
  "caption",
  "cite",
  "code",
  "dd",
  "del",
  "details",
  "dfn",
  "div",
  "dl",
  "dt",
  "em",
  "fieldset",
  "figcaption",
  "figure",
  "footer",
  "form",
  "h1",
  "h2",
  "h3",
  "h4",
  "h5",
  "h6",
  "header",
  "hgroup",
  "html",
  "i",
  "iframe",
  "img",
  "input",
  "ins",
  "kbd",
  "label",
  "legend",
  "li",
  "main",
  "mark",
  "menu",
  "nav",
  "object",
  "ol",
  "optgroup",
  "option",
  "p",
  "picture",
  "q",
  "quote",
  "samp",
  "section",
  "select",
  "source",
  "span",
  "strong",
  "summary",
  "sup",
  "table",
  "tbody",
  "td",
  "textarea",
  "tfoot",
  "th",
  "thead",
  "time",
  "tr",
  "ul",
  "var",
  "video"
], Ll = [
  "defs",
  "g",
  "marker",
  "mask",
  "pattern",
  "svg",
  "switch",
  "symbol",
  "feBlend",
  "feColorMatrix",
  "feComponentTransfer",
  "feComposite",
  "feConvolveMatrix",
  "feDiffuseLighting",
  "feDisplacementMap",
  "feFlood",
  "feGaussianBlur",
  "feImage",
  "feMerge",
  "feMorphology",
  "feOffset",
  "feSpecularLighting",
  "feTile",
  "feTurbulence",
  "linearGradient",
  "radialGradient",
  "stop",
  "circle",
  "ellipse",
  "image",
  "line",
  "path",
  "polygon",
  "polyline",
  "rect",
  "text",
  "use",
  "textPath",
  "tspan",
  "foreignObject",
  "clipPath"
], Bl = [
  ...Rl,
  ...Ll
], zl = [
  "any-hover",
  "any-pointer",
  "aspect-ratio",
  "color",
  "color-gamut",
  "color-index",
  "device-aspect-ratio",
  "device-height",
  "device-width",
  "display-mode",
  "forced-colors",
  "grid",
  "height",
  "hover",
  "inverted-colors",
  "monochrome",
  "orientation",
  "overflow-block",
  "overflow-inline",
  "pointer",
  "prefers-color-scheme",
  "prefers-contrast",
  "prefers-reduced-motion",
  "prefers-reduced-transparency",
  "resolution",
  "scan",
  "scripting",
  "update",
  "width",
  // TODO: find a better solution?
  "min-width",
  "max-width",
  "min-height",
  "max-height"
].sort().reverse(), Dl = [
  "active",
  "any-link",
  "blank",
  "checked",
  "current",
  "default",
  "defined",
  "dir",
  // dir()
  "disabled",
  "drop",
  "empty",
  "enabled",
  "first",
  "first-child",
  "first-of-type",
  "fullscreen",
  "future",
  "focus",
  "focus-visible",
  "focus-within",
  "has",
  // has()
  "host",
  // host or host()
  "host-context",
  // host-context()
  "hover",
  "indeterminate",
  "in-range",
  "invalid",
  "is",
  // is()
  "lang",
  // lang()
  "last-child",
  "last-of-type",
  "left",
  "link",
  "local-link",
  "not",
  // not()
  "nth-child",
  // nth-child()
  "nth-col",
  // nth-col()
  "nth-last-child",
  // nth-last-child()
  "nth-last-col",
  // nth-last-col()
  "nth-last-of-type",
  //nth-last-of-type()
  "nth-of-type",
  //nth-of-type()
  "only-child",
  "only-of-type",
  "optional",
  "out-of-range",
  "past",
  "placeholder-shown",
  "read-only",
  "read-write",
  "required",
  "right",
  "root",
  "scope",
  "target",
  "target-within",
  "user-invalid",
  "valid",
  "visited",
  "where"
  // where()
].sort().reverse(), Pl = [
  "after",
  "backdrop",
  "before",
  "cue",
  "cue-region",
  "first-letter",
  "first-line",
  "grammar-error",
  "marker",
  "part",
  "placeholder",
  "selection",
  "slotted",
  "spelling-error"
].sort().reverse(), Ul = [
  "accent-color",
  "align-content",
  "align-items",
  "align-self",
  "alignment-baseline",
  "all",
  "anchor-name",
  "animation",
  "animation-composition",
  "animation-delay",
  "animation-direction",
  "animation-duration",
  "animation-fill-mode",
  "animation-iteration-count",
  "animation-name",
  "animation-play-state",
  "animation-range",
  "animation-range-end",
  "animation-range-start",
  "animation-timeline",
  "animation-timing-function",
  "appearance",
  "aspect-ratio",
  "backdrop-filter",
  "backface-visibility",
  "background",
  "background-attachment",
  "background-blend-mode",
  "background-clip",
  "background-color",
  "background-image",
  "background-origin",
  "background-position",
  "background-position-x",
  "background-position-y",
  "background-repeat",
  "background-size",
  "baseline-shift",
  "block-size",
  "border",
  "border-block",
  "border-block-color",
  "border-block-end",
  "border-block-end-color",
  "border-block-end-style",
  "border-block-end-width",
  "border-block-start",
  "border-block-start-color",
  "border-block-start-style",
  "border-block-start-width",
  "border-block-style",
  "border-block-width",
  "border-bottom",
  "border-bottom-color",
  "border-bottom-left-radius",
  "border-bottom-right-radius",
  "border-bottom-style",
  "border-bottom-width",
  "border-collapse",
  "border-color",
  "border-end-end-radius",
  "border-end-start-radius",
  "border-image",
  "border-image-outset",
  "border-image-repeat",
  "border-image-slice",
  "border-image-source",
  "border-image-width",
  "border-inline",
  "border-inline-color",
  "border-inline-end",
  "border-inline-end-color",
  "border-inline-end-style",
  "border-inline-end-width",
  "border-inline-start",
  "border-inline-start-color",
  "border-inline-start-style",
  "border-inline-start-width",
  "border-inline-style",
  "border-inline-width",
  "border-left",
  "border-left-color",
  "border-left-style",
  "border-left-width",
  "border-radius",
  "border-right",
  "border-right-color",
  "border-right-style",
  "border-right-width",
  "border-spacing",
  "border-start-end-radius",
  "border-start-start-radius",
  "border-style",
  "border-top",
  "border-top-color",
  "border-top-left-radius",
  "border-top-right-radius",
  "border-top-style",
  "border-top-width",
  "border-width",
  "bottom",
  "box-align",
  "box-decoration-break",
  "box-direction",
  "box-flex",
  "box-flex-group",
  "box-lines",
  "box-ordinal-group",
  "box-orient",
  "box-pack",
  "box-shadow",
  "box-sizing",
  "break-after",
  "break-before",
  "break-inside",
  "caption-side",
  "caret-color",
  "clear",
  "clip",
  "clip-path",
  "clip-rule",
  "color",
  "color-interpolation",
  "color-interpolation-filters",
  "color-profile",
  "color-rendering",
  "color-scheme",
  "column-count",
  "column-fill",
  "column-gap",
  "column-rule",
  "column-rule-color",
  "column-rule-style",
  "column-rule-width",
  "column-span",
  "column-width",
  "columns",
  "contain",
  "contain-intrinsic-block-size",
  "contain-intrinsic-height",
  "contain-intrinsic-inline-size",
  "contain-intrinsic-size",
  "contain-intrinsic-width",
  "container",
  "container-name",
  "container-type",
  "content",
  "content-visibility",
  "counter-increment",
  "counter-reset",
  "counter-set",
  "cue",
  "cue-after",
  "cue-before",
  "cursor",
  "cx",
  "cy",
  "direction",
  "display",
  "dominant-baseline",
  "empty-cells",
  "enable-background",
  "field-sizing",
  "fill",
  "fill-opacity",
  "fill-rule",
  "filter",
  "flex",
  "flex-basis",
  "flex-direction",
  "flex-flow",
  "flex-grow",
  "flex-shrink",
  "flex-wrap",
  "float",
  "flood-color",
  "flood-opacity",
  "flow",
  "font",
  "font-display",
  "font-family",
  "font-feature-settings",
  "font-kerning",
  "font-language-override",
  "font-optical-sizing",
  "font-palette",
  "font-size",
  "font-size-adjust",
  "font-smooth",
  "font-smoothing",
  "font-stretch",
  "font-style",
  "font-synthesis",
  "font-synthesis-position",
  "font-synthesis-small-caps",
  "font-synthesis-style",
  "font-synthesis-weight",
  "font-variant",
  "font-variant-alternates",
  "font-variant-caps",
  "font-variant-east-asian",
  "font-variant-emoji",
  "font-variant-ligatures",
  "font-variant-numeric",
  "font-variant-position",
  "font-variation-settings",
  "font-weight",
  "forced-color-adjust",
  "gap",
  "glyph-orientation-horizontal",
  "glyph-orientation-vertical",
  "grid",
  "grid-area",
  "grid-auto-columns",
  "grid-auto-flow",
  "grid-auto-rows",
  "grid-column",
  "grid-column-end",
  "grid-column-start",
  "grid-gap",
  "grid-row",
  "grid-row-end",
  "grid-row-start",
  "grid-template",
  "grid-template-areas",
  "grid-template-columns",
  "grid-template-rows",
  "hanging-punctuation",
  "height",
  "hyphenate-character",
  "hyphenate-limit-chars",
  "hyphens",
  "icon",
  "image-orientation",
  "image-rendering",
  "image-resolution",
  "ime-mode",
  "initial-letter",
  "initial-letter-align",
  "inline-size",
  "inset",
  "inset-area",
  "inset-block",
  "inset-block-end",
  "inset-block-start",
  "inset-inline",
  "inset-inline-end",
  "inset-inline-start",
  "isolation",
  "justify-content",
  "justify-items",
  "justify-self",
  "kerning",
  "left",
  "letter-spacing",
  "lighting-color",
  "line-break",
  "line-height",
  "line-height-step",
  "list-style",
  "list-style-image",
  "list-style-position",
  "list-style-type",
  "margin",
  "margin-block",
  "margin-block-end",
  "margin-block-start",
  "margin-bottom",
  "margin-inline",
  "margin-inline-end",
  "margin-inline-start",
  "margin-left",
  "margin-right",
  "margin-top",
  "margin-trim",
  "marker",
  "marker-end",
  "marker-mid",
  "marker-start",
  "marks",
  "mask",
  "mask-border",
  "mask-border-mode",
  "mask-border-outset",
  "mask-border-repeat",
  "mask-border-slice",
  "mask-border-source",
  "mask-border-width",
  "mask-clip",
  "mask-composite",
  "mask-image",
  "mask-mode",
  "mask-origin",
  "mask-position",
  "mask-repeat",
  "mask-size",
  "mask-type",
  "masonry-auto-flow",
  "math-depth",
  "math-shift",
  "math-style",
  "max-block-size",
  "max-height",
  "max-inline-size",
  "max-width",
  "min-block-size",
  "min-height",
  "min-inline-size",
  "min-width",
  "mix-blend-mode",
  "nav-down",
  "nav-index",
  "nav-left",
  "nav-right",
  "nav-up",
  "none",
  "normal",
  "object-fit",
  "object-position",
  "offset",
  "offset-anchor",
  "offset-distance",
  "offset-path",
  "offset-position",
  "offset-rotate",
  "opacity",
  "order",
  "orphans",
  "outline",
  "outline-color",
  "outline-offset",
  "outline-style",
  "outline-width",
  "overflow",
  "overflow-anchor",
  "overflow-block",
  "overflow-clip-margin",
  "overflow-inline",
  "overflow-wrap",
  "overflow-x",
  "overflow-y",
  "overlay",
  "overscroll-behavior",
  "overscroll-behavior-block",
  "overscroll-behavior-inline",
  "overscroll-behavior-x",
  "overscroll-behavior-y",
  "padding",
  "padding-block",
  "padding-block-end",
  "padding-block-start",
  "padding-bottom",
  "padding-inline",
  "padding-inline-end",
  "padding-inline-start",
  "padding-left",
  "padding-right",
  "padding-top",
  "page",
  "page-break-after",
  "page-break-before",
  "page-break-inside",
  "paint-order",
  "pause",
  "pause-after",
  "pause-before",
  "perspective",
  "perspective-origin",
  "place-content",
  "place-items",
  "place-self",
  "pointer-events",
  "position",
  "position-anchor",
  "position-visibility",
  "print-color-adjust",
  "quotes",
  "r",
  "resize",
  "rest",
  "rest-after",
  "rest-before",
  "right",
  "rotate",
  "row-gap",
  "ruby-align",
  "ruby-position",
  "scale",
  "scroll-behavior",
  "scroll-margin",
  "scroll-margin-block",
  "scroll-margin-block-end",
  "scroll-margin-block-start",
  "scroll-margin-bottom",
  "scroll-margin-inline",
  "scroll-margin-inline-end",
  "scroll-margin-inline-start",
  "scroll-margin-left",
  "scroll-margin-right",
  "scroll-margin-top",
  "scroll-padding",
  "scroll-padding-block",
  "scroll-padding-block-end",
  "scroll-padding-block-start",
  "scroll-padding-bottom",
  "scroll-padding-inline",
  "scroll-padding-inline-end",
  "scroll-padding-inline-start",
  "scroll-padding-left",
  "scroll-padding-right",
  "scroll-padding-top",
  "scroll-snap-align",
  "scroll-snap-stop",
  "scroll-snap-type",
  "scroll-timeline",
  "scroll-timeline-axis",
  "scroll-timeline-name",
  "scrollbar-color",
  "scrollbar-gutter",
  "scrollbar-width",
  "shape-image-threshold",
  "shape-margin",
  "shape-outside",
  "shape-rendering",
  "speak",
  "speak-as",
  "src",
  // @font-face
  "stop-color",
  "stop-opacity",
  "stroke",
  "stroke-dasharray",
  "stroke-dashoffset",
  "stroke-linecap",
  "stroke-linejoin",
  "stroke-miterlimit",
  "stroke-opacity",
  "stroke-width",
  "tab-size",
  "table-layout",
  "text-align",
  "text-align-all",
  "text-align-last",
  "text-anchor",
  "text-combine-upright",
  "text-decoration",
  "text-decoration-color",
  "text-decoration-line",
  "text-decoration-skip",
  "text-decoration-skip-ink",
  "text-decoration-style",
  "text-decoration-thickness",
  "text-emphasis",
  "text-emphasis-color",
  "text-emphasis-position",
  "text-emphasis-style",
  "text-indent",
  "text-justify",
  "text-orientation",
  "text-overflow",
  "text-rendering",
  "text-shadow",
  "text-size-adjust",
  "text-transform",
  "text-underline-offset",
  "text-underline-position",
  "text-wrap",
  "text-wrap-mode",
  "text-wrap-style",
  "timeline-scope",
  "top",
  "touch-action",
  "transform",
  "transform-box",
  "transform-origin",
  "transform-style",
  "transition",
  "transition-behavior",
  "transition-delay",
  "transition-duration",
  "transition-property",
  "transition-timing-function",
  "translate",
  "unicode-bidi",
  "user-modify",
  "user-select",
  "vector-effect",
  "vertical-align",
  "view-timeline",
  "view-timeline-axis",
  "view-timeline-inset",
  "view-timeline-name",
  "view-transition-name",
  "visibility",
  "voice-balance",
  "voice-duration",
  "voice-family",
  "voice-pitch",
  "voice-range",
  "voice-rate",
  "voice-stress",
  "voice-volume",
  "white-space",
  "white-space-collapse",
  "widows",
  "width",
  "will-change",
  "word-break",
  "word-spacing",
  "word-wrap",
  "writing-mode",
  "x",
  "y",
  "z-index",
  "zoom"
].sort().reverse();
function ql(e) {
  const t = e.regex, n = Nl(e), s = { begin: /-(webkit|moz|ms|o)-(?=[a-z])/ }, r = "and or not only", o = /@-?\w[\w]*(-\w+)*/, l = "[a-zA-Z-][a-zA-Z0-9_-]*", i = [
    e.APOS_STRING_MODE,
    e.QUOTE_STRING_MODE
  ];
  return {
    name: "CSS",
    case_insensitive: !0,
    illegal: /[=|'\$]/,
    keywords: { keyframePosition: "from to" },
    classNameAliases: {
      // for visual continuity with `tag {}` and because we
      // don't have a great class for this?
      keyframePosition: "selector-tag"
    },
    contains: [
      n.BLOCK_COMMENT,
      s,
      // to recognize keyframe 40% etc which are outside the scope of our
      // attribute value mode
      n.CSS_NUMBER_MODE,
      {
        className: "selector-id",
        begin: /#[A-Za-z0-9_-]+/,
        relevance: 0
      },
      {
        className: "selector-class",
        begin: "\\." + l,
        relevance: 0
      },
      n.ATTRIBUTE_SELECTOR_MODE,
      {
        className: "selector-pseudo",
        variants: [
          { begin: ":(" + Dl.join("|") + ")" },
          { begin: ":(:)?(" + Pl.join("|") + ")" }
        ]
      },
      // we may actually need this (12/2020)
      // { // pseudo-selector params
      //   begin: /\(/,
      //   end: /\)/,
      //   contains: [ hljs.CSS_NUMBER_MODE ]
      // },
      n.CSS_VARIABLE,
      {
        className: "attribute",
        begin: "\\b(" + Ul.join("|") + ")\\b"
      },
      // attribute values
      {
        begin: /:/,
        end: /[;}{]/,
        contains: [
          n.BLOCK_COMMENT,
          n.HEXCOLOR,
          n.IMPORTANT,
          n.CSS_NUMBER_MODE,
          ...i,
          // needed to highlight these as strings and to avoid issues with
          // illegal characters that might be inside urls that would tigger the
          // languages illegal stack
          {
            begin: /(url|data-uri)\(/,
            end: /\)/,
            relevance: 0,
            // from keywords
            keywords: { built_in: "url data-uri" },
            contains: [
              ...i,
              {
                className: "string",
                // any character other than `)` as in `url()` will be the start
                // of a string, which ends with `)` (from the parent mode)
                begin: /[^)]/,
                endsWithParent: !0,
                excludeEnd: !0
              }
            ]
          },
          n.FUNCTION_DISPATCH
        ]
      },
      {
        begin: t.lookahead(/@/),
        end: "[{;]",
        relevance: 0,
        illegal: /:/,
        // break on Less variables @var: ...
        contains: [
          {
            className: "keyword",
            begin: o
          },
          {
            begin: /\s/,
            endsWithParent: !0,
            excludeEnd: !0,
            relevance: 0,
            keywords: {
              $pattern: /[a-z-]+/,
              keyword: r,
              attribute: zl.join(" ")
            },
            contains: [
              {
                begin: /[a-z-]+(?=:)/,
                className: "attribute"
              },
              ...i,
              n.CSS_NUMBER_MODE
            ]
          }
        ]
      },
      {
        className: "selector-tag",
        begin: "\\b(" + Bl.join("|") + ")\\b"
      }
    ]
  };
}
function jl(e) {
  const t = {
    className: "attr",
    begin: /"(\\.|[^\\"\r\n])*"(?=\s*:)/,
    relevance: 1.01
  }, n = {
    match: /[{}[\],:]/,
    className: "punctuation",
    relevance: 0
  }, s = [
    "true",
    "false",
    "null"
  ], r = {
    scope: "literal",
    beginKeywords: s.join(" ")
  };
  return {
    name: "JSON",
    aliases: ["jsonc"],
    keywords: {
      literal: s
    },
    contains: [
      t,
      n,
      e.QUOTE_STRING_MODE,
      r,
      e.C_NUMBER_MODE,
      e.C_LINE_COMMENT_MODE,
      e.C_BLOCK_COMMENT_MODE
    ],
    illegal: "\\S"
  };
}
function as(e) {
  const t = e.regex, n = {}, s = {
    begin: /\$\{/,
    end: /\}/,
    contains: [
      "self",
      {
        begin: /:-/,
        contains: [n]
      }
      // default values
    ]
  };
  Object.assign(n, {
    className: "variable",
    variants: [
      { begin: t.concat(
        /\$[\w\d#@][\w\d_]*/,
        // negative look-ahead tries to avoid matching patterns that are not
        // Perl at all like $ident$, @ident@, etc.
        "(?![\\w\\d])(?![$])"
      ) },
      s
    ]
  });
  const r = {
    className: "subst",
    begin: /\$\(/,
    end: /\)/,
    contains: [e.BACKSLASH_ESCAPE]
  }, o = e.inherit(
    e.COMMENT(),
    {
      match: [
        /(^|\s)/,
        /#.*$/
      ],
      scope: {
        2: "comment"
      }
    }
  ), l = {
    begin: /<<-?\s*(?=\w+)/,
    starts: { contains: [
      e.END_SAME_AS_BEGIN({
        begin: /(\w+)/,
        end: /(\w+)/,
        className: "string"
      })
    ] }
  }, i = {
    className: "string",
    begin: /"/,
    end: /"/,
    contains: [
      e.BACKSLASH_ESCAPE,
      n,
      r
    ]
  };
  r.contains.push(i);
  const a = {
    match: /\\"/
  }, u = {
    className: "string",
    begin: /'/,
    end: /'/
  }, c = {
    match: /\\'/
  }, p = {
    begin: /\$?\(\(/,
    end: /\)\)/,
    contains: [
      {
        begin: /\d+#[0-9a-f]+/,
        className: "number"
      },
      e.NUMBER_MODE,
      n
    ]
  }, d = [
    "fish",
    "bash",
    "zsh",
    "sh",
    "csh",
    "ksh",
    "tcsh",
    "dash",
    "scsh"
  ], _ = e.SHEBANG({
    binary: `(${d.join("|")})`,
    relevance: 10
  }), f = {
    className: "function",
    begin: /\w[\w\d_]*\s*\(\s*\)\s*\{/,
    returnBegin: !0,
    contains: [e.inherit(e.TITLE_MODE, { begin: /\w[\w\d_]*/ })],
    relevance: 0
  }, k = [
    "if",
    "then",
    "else",
    "elif",
    "fi",
    "time",
    "for",
    "while",
    "until",
    "in",
    "do",
    "done",
    "case",
    "esac",
    "coproc",
    "function",
    "select"
  ], $ = [
    "true",
    "false"
  ], I = { match: /(\/[a-z._-]+)+/ }, P = [
    "break",
    "cd",
    "continue",
    "eval",
    "exec",
    "exit",
    "export",
    "getopts",
    "hash",
    "pwd",
    "readonly",
    "return",
    "shift",
    "test",
    "times",
    "trap",
    "umask",
    "unset"
  ], R = [
    "alias",
    "bind",
    "builtin",
    "caller",
    "command",
    "declare",
    "echo",
    "enable",
    "help",
    "let",
    "local",
    "logout",
    "mapfile",
    "printf",
    "read",
    "readarray",
    "source",
    "sudo",
    "type",
    "typeset",
    "ulimit",
    "unalias"
  ], B = [
    "autoload",
    "bg",
    "bindkey",
    "bye",
    "cap",
    "chdir",
    "clone",
    "comparguments",
    "compcall",
    "compctl",
    "compdescribe",
    "compfiles",
    "compgroups",
    "compquote",
    "comptags",
    "comptry",
    "compvalues",
    "dirs",
    "disable",
    "disown",
    "echotc",
    "echoti",
    "emulate",
    "fc",
    "fg",
    "float",
    "functions",
    "getcap",
    "getln",
    "history",
    "integer",
    "jobs",
    "kill",
    "limit",
    "log",
    "noglob",
    "popd",
    "print",
    "pushd",
    "pushln",
    "rehash",
    "sched",
    "setcap",
    "setopt",
    "stat",
    "suspend",
    "ttyctl",
    "unfunction",
    "unhash",
    "unlimit",
    "unsetopt",
    "vared",
    "wait",
    "whence",
    "where",
    "which",
    "zcompile",
    "zformat",
    "zftp",
    "zle",
    "zmodload",
    "zparseopts",
    "zprof",
    "zpty",
    "zregexparse",
    "zsocket",
    "zstyle",
    "ztcp"
  ], F = [
    "chcon",
    "chgrp",
    "chown",
    "chmod",
    "cp",
    "dd",
    "df",
    "dir",
    "dircolors",
    "ln",
    "ls",
    "mkdir",
    "mkfifo",
    "mknod",
    "mktemp",
    "mv",
    "realpath",
    "rm",
    "rmdir",
    "shred",
    "sync",
    "touch",
    "truncate",
    "vdir",
    "b2sum",
    "base32",
    "base64",
    "cat",
    "cksum",
    "comm",
    "csplit",
    "cut",
    "expand",
    "fmt",
    "fold",
    "head",
    "join",
    "md5sum",
    "nl",
    "numfmt",
    "od",
    "paste",
    "ptx",
    "pr",
    "sha1sum",
    "sha224sum",
    "sha256sum",
    "sha384sum",
    "sha512sum",
    "shuf",
    "sort",
    "split",
    "sum",
    "tac",
    "tail",
    "tr",
    "tsort",
    "unexpand",
    "uniq",
    "wc",
    "arch",
    "basename",
    "chroot",
    "date",
    "dirname",
    "du",
    "echo",
    "env",
    "expr",
    "factor",
    // "false", // keyword literal already
    "groups",
    "hostid",
    "id",
    "link",
    "logname",
    "nice",
    "nohup",
    "nproc",
    "pathchk",
    "pinky",
    "printenv",
    "printf",
    "pwd",
    "readlink",
    "runcon",
    "seq",
    "sleep",
    "stat",
    "stdbuf",
    "stty",
    "tee",
    "test",
    "timeout",
    // "true", // keyword literal already
    "tty",
    "uname",
    "unlink",
    "uptime",
    "users",
    "who",
    "whoami",
    "yes"
  ];
  return {
    name: "Bash",
    aliases: [
      "sh",
      "zsh"
    ],
    keywords: {
      $pattern: /\b[a-z][a-z0-9._-]+\b/,
      keyword: k,
      literal: $,
      built_in: [
        ...P,
        ...R,
        // Shell modifiers
        "set",
        "shopt",
        ...B,
        ...F
      ]
    },
    contains: [
      _,
      // to catch known shells and boost relevancy
      e.SHEBANG(),
      // to catch unknown shells but still highlight the shebang
      f,
      p,
      o,
      l,
      I,
      i,
      a,
      u,
      c,
      n
    ]
  };
}
oe.registerLanguage("javascript", Zn);
oe.registerLanguage("js", Zn);
oe.registerLanguage("typescript", ss);
oe.registerLanguage("ts", ss);
oe.registerLanguage("csharp", rs);
oe.registerLanguage("cs", rs);
oe.registerLanguage("xml", Lt);
oe.registerLanguage("html", Lt);
oe.registerLanguage("vue", Lt);
oe.registerLanguage("css", ql);
oe.registerLanguage("json", jl);
oe.registerLanguage("bash", as);
oe.registerLanguage("sh", as);
const Hl = [
  "javascript",
  "typescript",
  "csharp",
  "xml",
  "css",
  "json",
  "bash"
], Vl = ["innerHTML"], Fl = ["innerHTML"], Gl = ["value"], Wl = /* @__PURE__ */ z({
  __name: "CodeBlock",
  props: {
    code: {},
    lang: {},
    filename: {},
    copy: { type: Boolean, default: !0 },
    readonly: { type: Boolean, default: !0 }
  },
  emits: ["update:code"],
  setup(e, { emit: t }) {
    const n = e, s = t, r = ae(!1), o = Y(() => {
      if (!n.lang || n.lang === "auto")
        return oe.highlightAuto(n.code, Hl).value;
      try {
        return oe.highlight(n.code, { language: n.lang, ignoreIllegals: !0 }).value;
      } catch {
        return l(n.code);
      }
    });
    function l(p) {
      return p.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    }
    const i = Y(
      () => !!n.filename || !!n.lang && n.lang !== "auto" || n.copy || !n.readonly
    ), a = ae(!1);
    async function u() {
      try {
        await navigator.clipboard.writeText(n.code), a.value = !0, setTimeout(() => {
          a.value = !1;
        }, 1200);
      } catch {
      }
    }
    function c(p) {
      s("update:code", p.target.value);
    }
    return (p, d) => (g(), b("div", {
      class: h(p.$style.block)
    }, [
      i.value ? (g(), b("div", {
        key: 0,
        class: h(p.$style.head)
      }, [
        e.filename ? (g(), b("span", {
          key: 0,
          class: h(p.$style.filename)
        }, x(e.filename), 3)) : L("", !0),
        e.lang && e.lang !== "auto" ? (g(), b("span", {
          key: 1,
          class: h(p.$style.lang)
        }, x(e.lang), 3)) : L("", !0),
        w("div", {
          class: h(p.$style.spacer)
        }, null, 2),
        e.readonly ? L("", !0) : (g(), b("button", {
          key: 2,
          type: "button",
          class: h([p.$style.btn, r.value && p.$style.btnOn]),
          onClick: d[0] || (d[0] = (_) => r.value = !r.value)
        }, x(r.value ? "done" : "edit"), 3)),
        e.copy ? (g(), b("button", {
          key: 3,
          type: "button",
          class: h(p.$style.btn),
          onClick: u
        }, x(a.value ? "copied" : "copy"), 3)) : L("", !0)
      ], 2)) : L("", !0),
      r.value ? (g(), b("div", {
        key: 2,
        class: h(p.$style.editorWrap)
      }, [
        w("pre", {
          class: h(p.$style.editorView),
          "aria-hidden": "true"
        }, [
          w("code", {
            class: "hljs",
            innerHTML: o.value
          }, null, 8, Fl),
          d[1] || (d[1] = fe())
        ], 2),
        w("textarea", {
          class: h(p.$style.editorInput),
          value: e.code,
          spellcheck: "false",
          autocorrect: "off",
          autocapitalize: "off",
          onInput: c
        }, null, 42, Gl)
      ], 2)) : (g(), b("pre", {
        key: 1,
        class: h(p.$style.pre)
      }, [
        w("code", {
          class: h(["hljs", p.$style.code]),
          innerHTML: o.value
        }, null, 10, Vl)
      ], 2))
    ], 2));
  }
}), Kl = "_block_aaqrg_2", Zl = "_head_aaqrg_9", Xl = "_filename_aaqrg_18", Yl = "_lang_aaqrg_24", Jl = "_spacer_aaqrg_35", Ql = "_btn_aaqrg_37", ec = "_btnOn_aaqrg_52", tc = "_pre_aaqrg_58", nc = "_code_aaqrg_65", sc = "_editorWrap_aaqrg_80", rc = "_editorView_aaqrg_86", ac = "_editorInput_aaqrg_86", oc = {
  block: Kl,
  head: Zl,
  filename: Xl,
  lang: Yl,
  spacer: Jl,
  btn: Ql,
  btnOn: ec,
  pre: tc,
  code: nc,
  editorWrap: sc,
  editorView: rc,
  editorInput: ac
}, ic = {
  $style: oc
}, em = /* @__PURE__ */ D(Wl, [["__cssModules", ic]]), lc = ["innerHTML"], cc = ["innerHTML"], dc = ["value"], uc = /* @__PURE__ */ z({
  __name: "TextVarBlock",
  props: {
    text: {},
    filename: {},
    copy: { type: Boolean, default: !0 },
    readonly: { type: Boolean, default: !0 }
  },
  emits: ["update:text"],
  setup(e, { emit: t }) {
    const n = e, s = t, r = ae(!1), o = /(\{\{\s*\$[a-zA-Z_][a-zA-Z0-9_]*\s*\}\})/g;
    function l(d) {
      return d.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    }
    const i = Y(() => n.text.split(o).map((_, f) => f % 2 === 1 ? `<span class="tvb-var">${l(_)}</span>` : l(_)).join("")), a = Y(
      () => !!n.filename || n.copy || !n.readonly
    ), u = ae(!1);
    async function c() {
      try {
        await navigator.clipboard.writeText(n.text), u.value = !0, setTimeout(() => {
          u.value = !1;
        }, 1200);
      } catch {
      }
    }
    function p(d) {
      s("update:text", d.target.value);
    }
    return (d, _) => (g(), b("div", {
      class: h(d.$style.block)
    }, [
      a.value ? (g(), b("div", {
        key: 0,
        class: h(d.$style.head)
      }, [
        e.filename ? (g(), b("span", {
          key: 0,
          class: h(d.$style.filename)
        }, x(e.filename), 3)) : L("", !0),
        w("div", {
          class: h(d.$style.spacer)
        }, null, 2),
        e.readonly ? L("", !0) : (g(), b("button", {
          key: 1,
          type: "button",
          class: h([d.$style.btn, r.value && d.$style.btnOn]),
          onClick: _[0] || (_[0] = (f) => r.value = !r.value)
        }, x(r.value ? "done" : "edit"), 3)),
        e.copy ? (g(), b("button", {
          key: 2,
          type: "button",
          class: h(d.$style.btn),
          onClick: c
        }, x(u.value ? "copied" : "copy"), 3)) : L("", !0)
      ], 2)) : L("", !0),
      r.value ? (g(), b("div", {
        key: 2,
        class: h(d.$style.editorWrap)
      }, [
        w("pre", {
          class: h(d.$style.editorView),
          "aria-hidden": "true"
        }, [
          w("span", { innerHTML: i.value }, null, 8, cc),
          _[1] || (_[1] = fe())
        ], 2),
        w("textarea", {
          class: h(d.$style.editorInput),
          value: e.text,
          spellcheck: "false",
          autocorrect: "off",
          autocapitalize: "off",
          onInput: p
        }, null, 42, dc)
      ], 2)) : (g(), b("pre", {
        key: 1,
        class: h(d.$style.pre)
      }, [
        w("span", {
          class: h(d.$style.text),
          innerHTML: i.value
        }, null, 10, lc)
      ], 2))
    ], 2));
  }
}), hc = "_block_1pww8_2", pc = "_head_1pww8_9", fc = "_filename_1pww8_18", gc = "_spacer_1pww8_24", yc = "_btn_1pww8_26", bc = "_btnOn_1pww8_41", mc = "_pre_1pww8_47", kc = "_text_1pww8_56", _c = "_editorView_1pww8_67", wc = "_editorWrap_1pww8_78", $c = "_editorInput_1pww8_84", vc = {
  block: hc,
  head: pc,
  filename: fc,
  spacer: gc,
  btn: yc,
  btnOn: bc,
  pre: mc,
  text: kc,
  editorView: _c,
  editorWrap: wc,
  editorInput: $c
}, Ec = {
  $style: vc
}, tm = /* @__PURE__ */ D(uc, [["__cssModules", Ec]]), Sc = { class: "seg-wrap" }, Tc = {
  class: "seg",
  role: "tablist"
}, xc = ["aria-selected", "onClick"], Mc = {
  key: 0,
  class: "seg-hint"
}, Ac = /* @__PURE__ */ z({
  __name: "Segmented",
  props: {
    modelValue: {},
    options: {}
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const n = e, s = t, r = () => {
      var o;
      return (o = n.options.find((l) => l.value === n.modelValue)) == null ? void 0 : o.hint;
    };
    return (o, l) => (g(), b("div", Sc, [
      w("div", Tc, [
        (g(!0), b(le, null, ye(e.options, (i) => (g(), b("button", {
          key: i.value,
          type: "button",
          role: "tab",
          "aria-selected": i.value === e.modelValue,
          class: h(["seg-btn", { active: i.value === e.modelValue }]),
          onClick: (a) => s("update:modelValue", i.value)
        }, x(i.label), 11, xc))), 128))
      ]),
      r() ? (g(), b("p", Mc, x(r()), 1)) : L("", !0)
    ]));
  }
}), nm = /* @__PURE__ */ D(Ac, [["__scopeId", "data-v-15af1c50"]]), Cc = { class: "varchips" }, Ic = { class: "vc-label" }, Oc = ["title", "onClick"], Nc = { class: "vc-tok" }, Rc = /* @__PURE__ */ z({
  __name: "VarChips",
  props: {
    vars: {},
    label: {}
  },
  emits: ["insert"],
  setup(e, { emit: t }) {
    const n = t;
    return (s, r) => (g(), b("div", Cc, [
      w("span", Ic, x(e.label ?? "Insert:"), 1),
      (g(!0), b(le, null, ye(e.vars, (o) => (g(), b("button", {
        key: o.token,
        type: "button",
        class: "vc-chip",
        title: o.desc ?? o.token,
        onClick: (l) => n("insert", o.token)
      }, [
        fe(x(o.label) + " ", 1),
        w("code", Nc, x(o.token), 1)
      ], 8, Oc))), 128))
    ]));
  }
}), sm = /* @__PURE__ */ D(Rc, [["__scopeId", "data-v-f3af8416"]]);
function Bt() {
  return { async: !1, breaks: !1, extensions: null, gfm: !0, hooks: null, pedantic: !1, renderer: null, silent: !1, tokenizer: null, walkTokens: null };
}
var De = Bt();
function os(e) {
  De = e;
}
var Ne = { exec: () => null };
function W(e, t = "") {
  let n = typeof e == "string" ? e : e.source, s = { replace: (r, o) => {
    let l = typeof o == "string" ? o : o.source;
    return l = l.replace(pe.caret, "$1"), n = n.replace(r, l), s;
  }, getRegex: () => new RegExp(n, t) };
  return s;
}
var Lc = ((e = "") => {
  try {
    return !!new RegExp("(?<=1)(?<!1)" + e);
  } catch {
    return !1;
  }
})(), pe = { codeRemoveIndent: /^(?: {1,4}| {0,3}\t)/gm, outputLinkReplace: /\\([\[\]])/g, indentCodeCompensation: /^(\s+)(?:```)/, beginningSpace: /^\s+/, endingHash: /#$/, startingSpaceChar: /^ /, endingSpaceChar: / $/, nonSpaceChar: /[^ ]/, newLineCharGlobal: /\n/g, tabCharGlobal: /\t/g, multipleSpaceGlobal: /\s+/g, blankLine: /^[ \t]*$/, doubleBlankLine: /\n[ \t]*\n[ \t]*$/, blockquoteStart: /^ {0,3}>/, blockquoteSetextReplace: /\n {0,3}((?:=+|-+) *)(?=\n|$)/g, blockquoteSetextReplace2: /^ {0,3}>[ \t]?/gm, listReplaceNesting: /^ {1,4}(?=( {4})*[^ ])/g, listIsTask: /^\[[ xX]\] +\S/, listReplaceTask: /^\[[ xX]\] +/, listTaskCheckbox: /\[[ xX]\]/, anyLine: /\n.*\n/, hrefBrackets: /^<(.*)>$/, tableDelimiter: /[:|]/, tableAlignChars: /^\||\| *$/g, tableRowBlankLine: /\n[ \t]*$/, tableAlignRight: /^ *-+: *$/, tableAlignCenter: /^ *:-+: *$/, tableAlignLeft: /^ *:-+ *$/, startATag: /^<a /i, endATag: /^<\/a>/i, startPreScriptTag: /^<(pre|code|kbd|script)(\s|>)/i, endPreScriptTag: /^<\/(pre|code|kbd|script)(\s|>)/i, startAngleBracket: /^</, endAngleBracket: />$/, pedanticHrefTitle: /^([^'"]*[^\s])\s+(['"])(.*)\2/, unicodeAlphaNumeric: /[\p{L}\p{N}]/u, escapeTest: /[&<>"']/, escapeReplace: /[&<>"']/g, escapeTestNoEncode: /[<>"']|&(?!(#\d{1,7}|#[Xx][a-fA-F0-9]{1,6}|\w+);)/, escapeReplaceNoEncode: /[<>"']|&(?!(#\d{1,7}|#[Xx][a-fA-F0-9]{1,6}|\w+);)/g, caret: /(^|[^\[])\^/g, percentDecode: /%25/g, findPipe: /\|/g, splitPipe: / \|/, slashPipe: /\\\|/g, carriageReturn: /\r\n|\r/g, spaceLine: /^ +$/gm, notSpaceStart: /^\S*/, endingNewline: /\n$/, listItemRegex: (e) => new RegExp(`^( {0,3}${e})((?:[	 ][^\\n]*)?(?:\\n|$))`), nextBulletRegex: (e) => new RegExp(`^ {0,${Math.min(3, e - 1)}}(?:[*+-]|\\d{1,9}[.)])((?:[ 	][^\\n]*)?(?:\\n|$))`), hrRegex: (e) => new RegExp(`^ {0,${Math.min(3, e - 1)}}((?:- *){3,}|(?:_ *){3,}|(?:\\* *){3,})(?:\\n+|$)`), fencesBeginRegex: (e) => new RegExp(`^ {0,${Math.min(3, e - 1)}}(?:\`\`\`|~~~)`), headingBeginRegex: (e) => new RegExp(`^ {0,${Math.min(3, e - 1)}}#`), htmlBeginRegex: (e) => new RegExp(`^ {0,${Math.min(3, e - 1)}}<(?:[a-z].*>|!--)`, "i"), blockquoteBeginRegex: (e) => new RegExp(`^ {0,${Math.min(3, e - 1)}}>`) }, Bc = /^(?:[ \t]*(?:\n|$))+/, zc = /^((?: {4}| {0,3}\t)[^\n]+(?:\n(?:[ \t]*(?:\n|$))*)?)+/, Dc = /^ {0,3}(`{3,}(?=[^`\n]*(?:\n|$))|~{3,})([^\n]*)(?:\n|$)(?:|([\s\S]*?)(?:\n|$))(?: {0,3}\1[~`]* *(?=\n|$)|$)/, Xe = /^ {0,3}((?:-[\t ]*){3,}|(?:_[ \t]*){3,}|(?:\*[ \t]*){3,})(?:\n+|$)/, Pc = /^ {0,3}(#{1,6})(?=\s|$)(.*)(?:\n+|$)/, zt = / {0,3}(?:[*+-]|\d{1,9}[.)])/, is = /^(?!bull |blockCode|fences|blockquote|heading|html|table)((?:.|\n(?!\s*?\n|bull |blockCode|fences|blockquote|heading|html|table))+?)\n {0,3}(=+|-+) *(?:\n+|$)/, ls = W(is).replace(/bull/g, zt).replace(/blockCode/g, /(?: {4}| {0,3}\t)/).replace(/fences/g, / {0,3}(?:`{3,}|~{3,})/).replace(/blockquote/g, / {0,3}>/).replace(/heading/g, / {0,3}#{1,6}/).replace(/html/g, / {0,3}<[^\n>]+>\n/).replace(/\|table/g, "").getRegex(), Uc = W(is).replace(/bull/g, zt).replace(/blockCode/g, /(?: {4}| {0,3}\t)/).replace(/fences/g, / {0,3}(?:`{3,}|~{3,})/).replace(/blockquote/g, / {0,3}>/).replace(/heading/g, / {0,3}#{1,6}/).replace(/html/g, / {0,3}<[^\n>]+>\n/).replace(/table/g, / {0,3}\|?(?:[:\- ]*\|)+[\:\- ]*\n/).getRegex(), Dt = /^([^\n]+(?:\n(?!hr|heading|lheading|blockquote|fences|list|html|table| +\n)[^\n]+)*)/, qc = /^[^\n]+/, Pt = /(?!\s*\])(?:\\[\s\S]|[^\[\]\\])+/, jc = W(/^ {0,3}\[(label)\]: *(?:\n[ \t]*)?([^<\s][^\s]*|<.*?>)(?:(?: +(?:\n[ \t]*)?| *\n[ \t]*)(title))? *(?:\n+|$)/).replace("label", Pt).replace("title", /(?:"(?:\\"?|[^"\\])*"|'[^'\n]*(?:\n[^'\n]+)*\n?'|\([^()]*\))/).getRegex(), Hc = W(/^(bull)([ \t][^\n]+?)?(?:\n|$)/).replace(/bull/g, zt).getRegex(), ut = "address|article|aside|base|basefont|blockquote|body|caption|center|col|colgroup|dd|details|dialog|dir|div|dl|dt|fieldset|figcaption|figure|footer|form|frame|frameset|h[1-6]|head|header|hr|html|iframe|legend|li|link|main|menu|menuitem|meta|nav|noframes|ol|optgroup|option|p|param|search|section|summary|table|tbody|td|tfoot|th|thead|title|tr|track|ul", Ut = /<!--(?:-?>|[\s\S]*?(?:-->|$))/, Vc = W("^ {0,3}(?:<(script|pre|style|textarea)[\\s>][\\s\\S]*?(?:</\\1>[^\\n]*\\n+|$)|comment[^\\n]*(\\n+|$)|<\\?[\\s\\S]*?(?:\\?>\\n*|$)|<![A-Z][\\s\\S]*?(?:>\\n*|$)|<!\\[CDATA\\[[\\s\\S]*?(?:\\]\\]>\\n*|$)|</?(tag)(?: +|\\n|/?>)[\\s\\S]*?(?:(?:\\n[ 	]*)+\\n|$)|<(?!script|pre|style|textarea)([a-z][\\w-]*)(?:attribute)*? */?>(?=[ \\t]*(?:\\n|$))[\\s\\S]*?(?:(?:\\n[ 	]*)+\\n|$)|</(?!script|pre|style|textarea)[a-z][\\w-]*\\s*>(?=[ \\t]*(?:\\n|$))[\\s\\S]*?(?:(?:\\n[ 	]*)+\\n|$))", "i").replace("comment", Ut).replace("tag", ut).replace("attribute", / +[a-zA-Z:_][\w.:-]*(?: *= *"[^"\n]*"| *= *'[^'\n]*'| *= *[^\s"'=<>`]+)?/).getRegex(), cs = W(Dt).replace("hr", Xe).replace("heading", " {0,3}#{1,6}(?:\\s|$)").replace("|lheading", "").replace("|table", "").replace("blockquote", " {0,3}>").replace("fences", " {0,3}(?:`{3,}(?=[^`\\n]*\\n)|~{3,})[^\\n]*\\n").replace("list", " {0,3}(?:[*+-]|1[.)])[ \\t]").replace("html", "</?(?:tag)(?: +|\\n|/?>)|<(?:script|pre|style|textarea|!--)").replace("tag", ut).getRegex(), Fc = W(/^( {0,3}> ?(paragraph|[^\n]*)(?:\n|$))+/).replace("paragraph", cs).getRegex(), qt = { blockquote: Fc, code: zc, def: jc, fences: Dc, heading: Pc, hr: Xe, html: Vc, lheading: ls, list: Hc, newline: Bc, paragraph: cs, table: Ne, text: qc }, un = W("^ *([^\\n ].*)\\n {0,3}((?:\\| *)?:?-+:? *(?:\\| *:?-+:? *)*(?:\\| *)?)(?:\\n((?:(?! *\\n|hr|heading|blockquote|code|fences|list|html).*(?:\\n|$))*)\\n*|$)").replace("hr", Xe).replace("heading", " {0,3}#{1,6}(?:\\s|$)").replace("blockquote", " {0,3}>").replace("code", "(?: {4}| {0,3}	)[^\\n]").replace("fences", " {0,3}(?:`{3,}(?=[^`\\n]*\\n)|~{3,})[^\\n]*\\n").replace("list", " {0,3}(?:[*+-]|1[.)])[ \\t]").replace("html", "</?(?:tag)(?: +|\\n|/?>)|<(?:script|pre|style|textarea|!--)").replace("tag", ut).getRegex(), Gc = { ...qt, lheading: Uc, table: un, paragraph: W(Dt).replace("hr", Xe).replace("heading", " {0,3}#{1,6}(?:\\s|$)").replace("|lheading", "").replace("table", un).replace("blockquote", " {0,3}>").replace("fences", " {0,3}(?:`{3,}(?=[^`\\n]*\\n)|~{3,})[^\\n]*\\n").replace("list", " {0,3}(?:[*+-]|1[.)])[ \\t]").replace("html", "</?(?:tag)(?: +|\\n|/?>)|<(?:script|pre|style|textarea|!--)").replace("tag", ut).getRegex() }, Wc = { ...qt, html: W(`^ *(?:comment *(?:\\n|\\s*$)|<(tag)[\\s\\S]+?</\\1> *(?:\\n{2,}|\\s*$)|<tag(?:"[^"]*"|'[^']*'|\\s[^'"/>\\s]*)*?/?> *(?:\\n{2,}|\\s*$))`).replace("comment", Ut).replace(/tag/g, "(?!(?:a|em|strong|small|s|cite|q|dfn|abbr|data|time|code|var|samp|kbd|sub|sup|i|b|u|mark|ruby|rt|rp|bdi|bdo|span|br|wbr|ins|del|img)\\b)\\w+(?!:|[^\\w\\s@]*@)\\b").getRegex(), def: /^ *\[([^\]]+)\]: *<?([^\s>]+)>?(?: +(["(][^\n]+[")]))? *(?:\n+|$)/, heading: /^(#{1,6})(.*)(?:\n+|$)/, fences: Ne, lheading: /^(.+?)\n {0,3}(=+|-+) *(?:\n+|$)/, paragraph: W(Dt).replace("hr", Xe).replace("heading", ` *#{1,6} *[^
]`).replace("lheading", ls).replace("|table", "").replace("blockquote", " {0,3}>").replace("|fences", "").replace("|list", "").replace("|html", "").replace("|tag", "").getRegex() }, Kc = /^\\([!"#$%&'()*+,\-./:;<=>?@\[\]\\^_`{|}~])/, Zc = /^(`+)([^`]|[^`][\s\S]*?[^`])\1(?!`)/, ds = /^( {2,}|\\)\n(?!\s*$)/, Xc = /^(`+|[^`])(?:(?= {2,}\n)|[\s\S]*?(?:(?=[\\<!\[`*_]|\b_|$)|[^ ](?= {2,}\n)))/, qe = /[\p{P}\p{S}]/u, ht = /[\s\p{P}\p{S}]/u, jt = /[^\s\p{P}\p{S}]/u, Yc = W(/^((?![*_])punctSpace)/, "u").replace(/punctSpace/g, ht).getRegex(), us = /(?!~)[\p{P}\p{S}]/u, Jc = /(?!~)[\s\p{P}\p{S}]/u, Qc = /(?:[^\s\p{P}\p{S}]|~)/u, ed = W(/link|precode-code|html/, "g").replace("link", /\[(?:[^\[\]`]|(?<a>`+)[^`]+\k<a>(?!`))*?\]\((?:\\[\s\S]|[^\\\(\)]|\((?:\\[\s\S]|[^\\\(\)])*\))*\)/).replace("precode-", Lc ? "(?<!`)()" : "(^^|[^`])").replace("code", /(?<b>`+)[^`]+\k<b>(?!`)/).replace("html", /<(?! )[^<>]*?>/).getRegex(), hs = /^(?:\*+(?:((?!\*)punct)|([^\s*]))?)|^_+(?:((?!_)punct)|([^\s_]))?/, td = W(hs, "u").replace(/punct/g, qe).getRegex(), nd = W(hs, "u").replace(/punct/g, us).getRegex(), ps = "^[^_*]*?__[^_*]*?\\*[^_*]*?(?=__)|[^*]+(?=[^*])|(?!\\*)punct(\\*+)(?=[\\s]|$)|notPunctSpace(\\*+)(?!\\*)(?=punctSpace|$)|(?!\\*)punctSpace(\\*+)(?=notPunctSpace)|[\\s](\\*+)(?!\\*)(?=punct)|(?!\\*)punct(\\*+)(?!\\*)(?=punct)|notPunctSpace(\\*+)(?=notPunctSpace)", sd = W(ps, "gu").replace(/notPunctSpace/g, jt).replace(/punctSpace/g, ht).replace(/punct/g, qe).getRegex(), rd = W(ps, "gu").replace(/notPunctSpace/g, Qc).replace(/punctSpace/g, Jc).replace(/punct/g, us).getRegex(), ad = W("^[^_*]*?\\*\\*[^_*]*?_[^_*]*?(?=\\*\\*)|[^_]+(?=[^_])|(?!_)punct(_+)(?=[\\s]|$)|notPunctSpace(_+)(?!_)(?=punctSpace|$)|(?!_)punctSpace(_+)(?=notPunctSpace)|[\\s](_+)(?!_)(?=punct)|(?!_)punct(_+)(?!_)(?=punct)", "gu").replace(/notPunctSpace/g, jt).replace(/punctSpace/g, ht).replace(/punct/g, qe).getRegex(), od = W(/^~~?(?:((?!~)punct)|[^\s~])/, "u").replace(/punct/g, qe).getRegex(), id = "^[^~]+(?=[^~])|(?!~)punct(~~?)(?=[\\s]|$)|notPunctSpace(~~?)(?!~)(?=punctSpace|$)|(?!~)punctSpace(~~?)(?=notPunctSpace)|[\\s](~~?)(?!~)(?=punct)|(?!~)punct(~~?)(?!~)(?=punct)|notPunctSpace(~~?)(?=notPunctSpace)", ld = W(id, "gu").replace(/notPunctSpace/g, jt).replace(/punctSpace/g, ht).replace(/punct/g, qe).getRegex(), cd = W(/\\(punct)/, "gu").replace(/punct/g, qe).getRegex(), dd = W(/^<(scheme:[^\s\x00-\x1f<>]*|email)>/).replace("scheme", /[a-zA-Z][a-zA-Z0-9+.-]{1,31}/).replace("email", /[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+(@)[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+(?![-_])/).getRegex(), ud = W(Ut).replace("(?:-->|$)", "-->").getRegex(), hd = W("^comment|^</[a-zA-Z][\\w:-]*\\s*>|^<[a-zA-Z][\\w-]*(?:attribute)*?\\s*/?>|^<\\?[\\s\\S]*?\\?>|^<![a-zA-Z]+\\s[\\s\\S]*?>|^<!\\[CDATA\\[[\\s\\S]*?\\]\\]>").replace("comment", ud).replace("attribute", /\s+[a-zA-Z:_][\w.:-]*(?:\s*=\s*"[^"]*"|\s*=\s*'[^']*'|\s*=\s*[^\s"'=<>`]+)?/).getRegex(), at = /(?:\[(?:\\[\s\S]|[^\[\]\\])*\]|\\[\s\S]|`+(?!`)[^`]*?`+(?!`)|``+(?=\])|[^\[\]\\`])*?/, pd = W(/^!?\[(label)\]\(\s*(href)(?:(?:[ \t]+(?:\n[ \t]*)?|\n[ \t]*)(title))?\s*\)/).replace("label", at).replace("href", /<(?:\\.|[^\n<>\\])+>|[^ \t\n\x00-\x1f]*/).replace("title", /"(?:\\"?|[^"\\])*"|'(?:\\'?|[^'\\])*'|\((?:\\\)?|[^)\\])*\)/).getRegex(), fs = W(/^!?\[(label)\]\[(ref)\]/).replace("label", at).replace("ref", Pt).getRegex(), gs = W(/^!?\[(ref)\](?:\[\])?/).replace("ref", Pt).getRegex(), fd = W("reflink|nolink(?!\\()", "g").replace("reflink", fs).replace("nolink", gs).getRegex(), hn = /[hH][tT][tT][pP][sS]?|[fF][tT][pP]/, Ht = { _backpedal: Ne, anyPunctuation: cd, autolink: dd, blockSkip: ed, br: ds, code: Zc, del: Ne, delLDelim: Ne, delRDelim: Ne, emStrongLDelim: td, emStrongRDelimAst: sd, emStrongRDelimUnd: ad, escape: Kc, link: pd, nolink: gs, punctuation: Yc, reflink: fs, reflinkSearch: fd, tag: hd, text: Xc, url: Ne }, gd = { ...Ht, link: W(/^!?\[(label)\]\((.*?)\)/).replace("label", at).getRegex(), reflink: W(/^!?\[(label)\]\s*\[([^\]]*)\]/).replace("label", at).getRegex() }, Tt = { ...Ht, emStrongRDelimAst: rd, emStrongLDelim: nd, delLDelim: od, delRDelim: ld, url: W(/^((?:protocol):\/\/|www\.)(?:[a-zA-Z0-9\-]+\.?)+[^\s<]*|^email/).replace("protocol", hn).replace("email", /[A-Za-z0-9._+-]+(@)[a-zA-Z0-9-_]+(?:\.[a-zA-Z0-9-_]*[a-zA-Z0-9])+(?![-_])/).getRegex(), _backpedal: /(?:[^?!.,:;*_'"~()&]+|\([^)]*\)|&(?![a-zA-Z0-9]+;$)|[?!.,:;*_'"~)]+(?!$))+/, del: /^(~~?)(?=[^\s~])((?:\\[\s\S]|[^\\])*?(?:\\[\s\S]|[^\s~\\]))\1(?=[^~]|$)/, text: W(/^([`~]+|[^`~])(?:(?= {2,}\n)|(?=[a-zA-Z0-9.!#$%&'*+\/=?_`{\|}~-]+@)|[\s\S]*?(?:(?=[\\<!\[`*~_]|\b_|protocol:\/\/|www\.|$)|[^ ](?= {2,}\n)|[^a-zA-Z0-9.!#$%&'*+\/=?_`{\|}~-](?=[a-zA-Z0-9.!#$%&'*+\/=?_`{\|}~-]+@)))/).replace("protocol", hn).getRegex() }, yd = { ...Tt, br: W(ds).replace("{2,}", "*").getRegex(), text: W(Tt.text).replace("\\b_", "\\b_| {2,}\\n").replace(/\{2,\}/g, "*").getRegex() }, et = { normal: qt, gfm: Gc, pedantic: Wc }, je = { normal: Ht, gfm: Tt, breaks: yd, pedantic: gd }, bd = { "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }, pn = (e) => bd[e];
function Ee(e, t) {
  if (t) {
    if (pe.escapeTest.test(e)) return e.replace(pe.escapeReplace, pn);
  } else if (pe.escapeTestNoEncode.test(e)) return e.replace(pe.escapeReplaceNoEncode, pn);
  return e;
}
function fn(e) {
  try {
    e = encodeURI(e).replace(pe.percentDecode, "%");
  } catch {
    return null;
  }
  return e;
}
function gn(e, t) {
  var o;
  let n = e.replace(pe.findPipe, (l, i, a) => {
    let u = !1, c = i;
    for (; --c >= 0 && a[c] === "\\"; ) u = !u;
    return u ? "|" : " |";
  }), s = n.split(pe.splitPipe), r = 0;
  if (s[0].trim() || s.shift(), s.length > 0 && !((o = s.at(-1)) != null && o.trim()) && s.pop(), t) if (s.length > t) s.splice(t);
  else for (; s.length < t; ) s.push("");
  for (; r < s.length; r++) s[r] = s[r].trim().replace(pe.slashPipe, "|");
  return s;
}
function Ae(e, t, n) {
  let s = e.length;
  if (s === 0) return "";
  let r = 0;
  for (; r < s && e.charAt(s - r - 1) === t; )
    r++;
  return e.slice(0, s - r);
}
function yn(e) {
  let t = e.split(`
`), n = t.length - 1;
  for (; n >= 0 && pe.blankLine.test(t[n]); ) n--;
  return t.length - n <= 2 ? e : t.slice(0, n + 1).join(`
`);
}
function md(e, t) {
  if (e.indexOf(t[1]) === -1) return -1;
  let n = 0;
  for (let s = 0; s < e.length; s++) if (e[s] === "\\") s++;
  else if (e[s] === t[0]) n++;
  else if (e[s] === t[1] && (n--, n < 0)) return s;
  return n > 0 ? -2 : -1;
}
function kd(e, t = 0) {
  let n = t, s = "";
  for (let r of e) if (r === "	") {
    let o = 4 - n % 4;
    s += " ".repeat(o), n += o;
  } else s += r, n++;
  return s;
}
function bn(e, t, n, s, r) {
  let o = t.href, l = t.title || null, i = e[1].replace(r.other.outputLinkReplace, "$1");
  s.state.inLink = !0;
  let a = { type: e[0].charAt(0) === "!" ? "image" : "link", raw: n, href: o, title: l, text: i, tokens: s.inlineTokens(i) };
  return s.state.inLink = !1, a;
}
function _d(e, t, n) {
  let s = e.match(n.other.indentCodeCompensation);
  if (s === null) return t;
  let r = s[1];
  return t.split(`
`).map((o) => {
    let l = o.match(n.other.beginningSpace);
    if (l === null) return o;
    let [i] = l;
    return i.length >= r.length ? o.slice(r.length) : o;
  }).join(`
`);
}
var ot = class {
  constructor(e) {
    G(this, "options");
    G(this, "rules");
    G(this, "lexer");
    this.options = e || De;
  }
  space(e) {
    let t = this.rules.block.newline.exec(e);
    if (t && t[0].length > 0) return { type: "space", raw: t[0] };
  }
  code(e) {
    let t = this.rules.block.code.exec(e);
    if (t) {
      let n = this.options.pedantic ? t[0] : yn(t[0]), s = n.replace(this.rules.other.codeRemoveIndent, "");
      return { type: "code", raw: n, codeBlockStyle: "indented", text: s };
    }
  }
  fences(e) {
    let t = this.rules.block.fences.exec(e);
    if (t) {
      let n = t[0], s = _d(n, t[3] || "", this.rules);
      return { type: "code", raw: n, lang: t[2] ? t[2].trim().replace(this.rules.inline.anyPunctuation, "$1") : t[2], text: s };
    }
  }
  heading(e) {
    let t = this.rules.block.heading.exec(e);
    if (t) {
      let n = t[2].trim();
      if (this.rules.other.endingHash.test(n)) {
        let s = Ae(n, "#");
        (this.options.pedantic || !s || this.rules.other.endingSpaceChar.test(s)) && (n = s.trim());
      }
      return { type: "heading", raw: Ae(t[0], `
`), depth: t[1].length, text: n, tokens: this.lexer.inline(n) };
    }
  }
  hr(e) {
    let t = this.rules.block.hr.exec(e);
    if (t) return { type: "hr", raw: Ae(t[0], `
`) };
  }
  blockquote(e) {
    let t = this.rules.block.blockquote.exec(e);
    if (t) {
      let n = Ae(t[0], `
`).split(`
`), s = "", r = "", o = [];
      for (; n.length > 0; ) {
        let l = !1, i = [], a;
        for (a = 0; a < n.length; a++) if (this.rules.other.blockquoteStart.test(n[a])) i.push(n[a]), l = !0;
        else if (!l) i.push(n[a]);
        else break;
        n = n.slice(a);
        let u = i.join(`
`), c = u.replace(this.rules.other.blockquoteSetextReplace, `
    $1`).replace(this.rules.other.blockquoteSetextReplace2, "");
        s = s ? `${s}
${u}` : u, r = r ? `${r}
${c}` : c;
        let p = this.lexer.state.top;
        if (this.lexer.state.top = !0, this.lexer.blockTokens(c, o, !0), this.lexer.state.top = p, n.length === 0) break;
        let d = o.at(-1);
        if ((d == null ? void 0 : d.type) === "code") break;
        if ((d == null ? void 0 : d.type) === "blockquote") {
          let _ = d, f = _.raw + `
` + n.join(`
`), k = this.blockquote(f);
          o[o.length - 1] = k, s = s.substring(0, s.length - _.raw.length) + k.raw, r = r.substring(0, r.length - _.text.length) + k.text;
          break;
        } else if ((d == null ? void 0 : d.type) === "list") {
          let _ = d, f = _.raw + `
` + n.join(`
`), k = this.list(f);
          o[o.length - 1] = k, s = s.substring(0, s.length - d.raw.length) + k.raw, r = r.substring(0, r.length - _.raw.length) + k.raw, n = f.substring(o.at(-1).raw.length).split(`
`);
          continue;
        }
      }
      return { type: "blockquote", raw: s, tokens: o, text: r };
    }
  }
  list(e) {
    let t = this.rules.block.list.exec(e);
    if (t) {
      let n = t[1].trim(), s = n.length > 1, r = { type: "list", raw: "", ordered: s, start: s ? +n.slice(0, -1) : "", loose: !1, items: [] };
      n = s ? `\\d{1,9}\\${n.slice(-1)}` : `\\${n}`, this.options.pedantic && (n = s ? n : "[*+-]");
      let o = this.rules.other.listItemRegex(n), l = !1;
      for (; e; ) {
        let a = !1, u = "", c = "";
        if (!(t = o.exec(e)) || this.rules.block.hr.test(e)) break;
        u = t[0], e = e.substring(u.length);
        let p = kd(t[2].split(`
`, 1)[0], t[1].length), d = e.split(`
`, 1)[0], _ = !p.trim(), f = 0;
        if (this.options.pedantic ? (f = 2, c = p.trimStart()) : _ ? f = t[1].length + 1 : (f = p.search(this.rules.other.nonSpaceChar), f = f > 4 ? 1 : f, c = p.slice(f), f += t[1].length), _ && this.rules.other.blankLine.test(d) && (u += d + `
`, e = e.substring(d.length + 1), a = !0), !a) {
          let k = this.rules.other.nextBulletRegex(f), $ = this.rules.other.hrRegex(f), I = this.rules.other.fencesBeginRegex(f), P = this.rules.other.headingBeginRegex(f), R = this.rules.other.htmlBeginRegex(f), B = this.rules.other.blockquoteBeginRegex(f);
          for (; e; ) {
            let F = e.split(`
`, 1)[0], M;
            if (d = F, this.options.pedantic ? (d = d.replace(this.rules.other.listReplaceNesting, "  "), M = d) : M = d.replace(this.rules.other.tabCharGlobal, "    "), I.test(d) || P.test(d) || R.test(d) || B.test(d) || k.test(d) || $.test(d)) break;
            if (M.search(this.rules.other.nonSpaceChar) >= f || !d.trim()) c += `
` + M.slice(f);
            else {
              if (_ || p.replace(this.rules.other.tabCharGlobal, "    ").search(this.rules.other.nonSpaceChar) >= 4 || I.test(p) || P.test(p) || $.test(p)) break;
              c += `
` + d;
            }
            _ = !d.trim(), u += F + `
`, e = e.substring(F.length + 1), p = M.slice(f);
          }
        }
        r.loose || (l ? r.loose = !0 : this.rules.other.doubleBlankLine.test(u) && (l = !0)), r.items.push({ type: "list_item", raw: u, task: !!this.options.gfm && this.rules.other.listIsTask.test(c), loose: !1, text: c, tokens: [] }), r.raw += u;
      }
      let i = r.items.at(-1);
      if (i) i.raw = i.raw.trimEnd(), i.text = i.text.trimEnd();
      else return;
      r.raw = r.raw.trimEnd();
      for (let a of r.items) {
        this.lexer.state.top = !1, a.tokens = this.lexer.blockTokens(a.text, []);
        let u = a.tokens[0];
        if (a.task && ((u == null ? void 0 : u.type) === "text" || (u == null ? void 0 : u.type) === "paragraph")) {
          a.text = a.text.replace(this.rules.other.listReplaceTask, ""), u.raw = u.raw.replace(this.rules.other.listReplaceTask, ""), u.text = u.text.replace(this.rules.other.listReplaceTask, "");
          for (let p = this.lexer.inlineQueue.length - 1; p >= 0; p--) if (this.rules.other.listIsTask.test(this.lexer.inlineQueue[p].src)) {
            this.lexer.inlineQueue[p].src = this.lexer.inlineQueue[p].src.replace(this.rules.other.listReplaceTask, "");
            break;
          }
          let c = this.rules.other.listTaskCheckbox.exec(a.raw);
          if (c) {
            let p = { type: "checkbox", raw: c[0] + " ", checked: c[0] !== "[ ]" };
            a.checked = p.checked, r.loose ? a.tokens[0] && ["paragraph", "text"].includes(a.tokens[0].type) && "tokens" in a.tokens[0] && a.tokens[0].tokens ? (a.tokens[0].raw = p.raw + a.tokens[0].raw, a.tokens[0].text = p.raw + a.tokens[0].text, a.tokens[0].tokens.unshift(p)) : a.tokens.unshift({ type: "paragraph", raw: p.raw, text: p.raw, tokens: [p] }) : a.tokens.unshift(p);
          }
        } else a.task && (a.task = !1);
        if (!r.loose) {
          let c = a.tokens.filter((d) => d.type === "space"), p = c.length > 0 && c.some((d) => this.rules.other.anyLine.test(d.raw));
          r.loose = p;
        }
      }
      if (r.loose) for (let a of r.items) {
        a.loose = !0;
        for (let u of a.tokens) u.type === "text" && (u.type = "paragraph");
      }
      return r;
    }
  }
  html(e) {
    let t = this.rules.block.html.exec(e);
    if (t) {
      let n = yn(t[0]);
      return { type: "html", block: !0, raw: n, pre: t[1] === "pre" || t[1] === "script" || t[1] === "style", text: n };
    }
  }
  def(e) {
    let t = this.rules.block.def.exec(e);
    if (t) {
      let n = t[1].toLowerCase().replace(this.rules.other.multipleSpaceGlobal, " "), s = t[2] ? t[2].replace(this.rules.other.hrefBrackets, "$1").replace(this.rules.inline.anyPunctuation, "$1") : "", r = t[3] ? t[3].substring(1, t[3].length - 1).replace(this.rules.inline.anyPunctuation, "$1") : t[3];
      return { type: "def", tag: n, raw: Ae(t[0], `
`), href: s, title: r };
    }
  }
  table(e) {
    var l;
    let t = this.rules.block.table.exec(e);
    if (!t || !this.rules.other.tableDelimiter.test(t[2])) return;
    let n = gn(t[1]), s = t[2].replace(this.rules.other.tableAlignChars, "").split("|"), r = (l = t[3]) != null && l.trim() ? t[3].replace(this.rules.other.tableRowBlankLine, "").split(`
`) : [], o = { type: "table", raw: Ae(t[0], `
`), header: [], align: [], rows: [] };
    if (n.length === s.length) {
      for (let i of s) this.rules.other.tableAlignRight.test(i) ? o.align.push("right") : this.rules.other.tableAlignCenter.test(i) ? o.align.push("center") : this.rules.other.tableAlignLeft.test(i) ? o.align.push("left") : o.align.push(null);
      for (let i = 0; i < n.length; i++) o.header.push({ text: n[i], tokens: this.lexer.inline(n[i]), header: !0, align: o.align[i] });
      for (let i of r) o.rows.push(gn(i, o.header.length).map((a, u) => ({ text: a, tokens: this.lexer.inline(a), header: !1, align: o.align[u] })));
      return o;
    }
  }
  lheading(e) {
    let t = this.rules.block.lheading.exec(e);
    if (t) {
      let n = t[1].trim();
      return { type: "heading", raw: Ae(t[0], `
`), depth: t[2].charAt(0) === "=" ? 1 : 2, text: n, tokens: this.lexer.inline(n) };
    }
  }
  paragraph(e) {
    let t = this.rules.block.paragraph.exec(e);
    if (t) {
      let n = t[1].charAt(t[1].length - 1) === `
` ? t[1].slice(0, -1) : t[1];
      return { type: "paragraph", raw: t[0], text: n, tokens: this.lexer.inline(n) };
    }
  }
  text(e) {
    let t = this.rules.block.text.exec(e);
    if (t) return { type: "text", raw: t[0], text: t[0], tokens: this.lexer.inline(t[0]) };
  }
  escape(e) {
    let t = this.rules.inline.escape.exec(e);
    if (t) return { type: "escape", raw: t[0], text: t[1] };
  }
  tag(e) {
    let t = this.rules.inline.tag.exec(e);
    if (t) return !this.lexer.state.inLink && this.rules.other.startATag.test(t[0]) ? this.lexer.state.inLink = !0 : this.lexer.state.inLink && this.rules.other.endATag.test(t[0]) && (this.lexer.state.inLink = !1), !this.lexer.state.inRawBlock && this.rules.other.startPreScriptTag.test(t[0]) ? this.lexer.state.inRawBlock = !0 : this.lexer.state.inRawBlock && this.rules.other.endPreScriptTag.test(t[0]) && (this.lexer.state.inRawBlock = !1), { type: "html", raw: t[0], inLink: this.lexer.state.inLink, inRawBlock: this.lexer.state.inRawBlock, block: !1, text: t[0] };
  }
  link(e) {
    let t = this.rules.inline.link.exec(e);
    if (t) {
      let n = t[2].trim();
      if (!this.options.pedantic && this.rules.other.startAngleBracket.test(n)) {
        if (!this.rules.other.endAngleBracket.test(n)) return;
        let o = Ae(n.slice(0, -1), "\\");
        if ((n.length - o.length) % 2 === 0) return;
      } else {
        let o = md(t[2], "()");
        if (o === -2) return;
        if (o > -1) {
          let l = (t[0].indexOf("!") === 0 ? 5 : 4) + t[1].length + o;
          t[2] = t[2].substring(0, o), t[0] = t[0].substring(0, l).trim(), t[3] = "";
        }
      }
      let s = t[2], r = "";
      if (this.options.pedantic) {
        let o = this.rules.other.pedanticHrefTitle.exec(s);
        o && (s = o[1], r = o[3]);
      } else r = t[3] ? t[3].slice(1, -1) : "";
      return s = s.trim(), this.rules.other.startAngleBracket.test(s) && (this.options.pedantic && !this.rules.other.endAngleBracket.test(n) ? s = s.slice(1) : s = s.slice(1, -1)), bn(t, { href: s && s.replace(this.rules.inline.anyPunctuation, "$1"), title: r && r.replace(this.rules.inline.anyPunctuation, "$1") }, t[0], this.lexer, this.rules);
    }
  }
  reflink(e, t) {
    let n;
    if ((n = this.rules.inline.reflink.exec(e)) || (n = this.rules.inline.nolink.exec(e))) {
      let s = (n[2] || n[1]).replace(this.rules.other.multipleSpaceGlobal, " "), r = t[s.toLowerCase()];
      if (!r) {
        let o = n[0].charAt(0);
        return { type: "text", raw: o, text: o };
      }
      return bn(n, r, n[0], this.lexer, this.rules);
    }
  }
  emStrong(e, t, n = "") {
    let s = this.rules.inline.emStrongLDelim.exec(e);
    if (!(!s || !s[1] && !s[2] && !s[3] && !s[4] || s[4] && n.match(this.rules.other.unicodeAlphaNumeric)) && (!(s[1] || s[3]) || !n || this.rules.inline.punctuation.exec(n))) {
      let r = [...s[0]].length - 1, o, l, i = r, a = 0, u = s[0][0] === "*" ? this.rules.inline.emStrongRDelimAst : this.rules.inline.emStrongRDelimUnd;
      for (u.lastIndex = 0, t = t.slice(-1 * e.length + r); (s = u.exec(t)) !== null; ) {
        if (o = s[1] || s[2] || s[3] || s[4] || s[5] || s[6], !o) continue;
        if (l = [...o].length, s[3] || s[4]) {
          i += l;
          continue;
        } else if ((s[5] || s[6]) && r % 3 && !((r + l) % 3)) {
          a += l;
          continue;
        }
        if (i -= l, i > 0) continue;
        l = Math.min(l, l + i + a);
        let c = [...s[0]][0].length, p = e.slice(0, r + s.index + c + l);
        if (Math.min(r, l) % 2) {
          let _ = p.slice(1, -1);
          return { type: "em", raw: p, text: _, tokens: this.lexer.inlineTokens(_) };
        }
        let d = p.slice(2, -2);
        return { type: "strong", raw: p, text: d, tokens: this.lexer.inlineTokens(d) };
      }
    }
  }
  codespan(e) {
    let t = this.rules.inline.code.exec(e);
    if (t) {
      let n = t[2].replace(this.rules.other.newLineCharGlobal, " "), s = this.rules.other.nonSpaceChar.test(n), r = this.rules.other.startingSpaceChar.test(n) && this.rules.other.endingSpaceChar.test(n);
      return s && r && (n = n.substring(1, n.length - 1)), { type: "codespan", raw: t[0], text: n };
    }
  }
  br(e) {
    let t = this.rules.inline.br.exec(e);
    if (t) return { type: "br", raw: t[0] };
  }
  del(e, t, n = "") {
    let s = this.rules.inline.delLDelim.exec(e);
    if (s && (!s[1] || !n || this.rules.inline.punctuation.exec(n))) {
      let r = [...s[0]].length - 1, o, l, i = r, a = this.rules.inline.delRDelim;
      for (a.lastIndex = 0, t = t.slice(-1 * e.length + r); (s = a.exec(t)) !== null; ) {
        if (o = s[1] || s[2] || s[3] || s[4] || s[5] || s[6], !o || (l = [...o].length, l !== r)) continue;
        if (s[3] || s[4]) {
          i += l;
          continue;
        }
        if (i -= l, i > 0) continue;
        l = Math.min(l, l + i);
        let u = [...s[0]][0].length, c = e.slice(0, r + s.index + u + l), p = c.slice(r, -r);
        return { type: "del", raw: c, text: p, tokens: this.lexer.inlineTokens(p) };
      }
    }
  }
  autolink(e) {
    let t = this.rules.inline.autolink.exec(e);
    if (t) {
      let n, s;
      return t[2] === "@" ? (n = t[1], s = "mailto:" + n) : (n = t[1], s = n), { type: "link", raw: t[0], text: n, href: s, tokens: [{ type: "text", raw: n, text: n }] };
    }
  }
  url(e) {
    var n;
    let t;
    if (t = this.rules.inline.url.exec(e)) {
      let s, r;
      if (t[2] === "@") s = t[0], r = "mailto:" + s;
      else {
        let o;
        do
          o = t[0], t[0] = ((n = this.rules.inline._backpedal.exec(t[0])) == null ? void 0 : n[0]) ?? "";
        while (o !== t[0]);
        s = t[0], t[1] === "www." ? r = "http://" + t[0] : r = t[0];
      }
      return { type: "link", raw: t[0], text: s, href: r, tokens: [{ type: "text", raw: s, text: s }] };
    }
  }
  inlineText(e) {
    let t = this.rules.inline.text.exec(e);
    if (t) {
      let n = this.lexer.state.inRawBlock;
      return { type: "text", raw: t[0], text: t[0], escaped: n };
    }
  }
}, me = class xt {
  constructor(t) {
    G(this, "tokens");
    G(this, "options");
    G(this, "state");
    G(this, "inlineQueue");
    G(this, "tokenizer");
    this.tokens = [], this.tokens.links = /* @__PURE__ */ Object.create(null), this.options = t || De, this.options.tokenizer = this.options.tokenizer || new ot(), this.tokenizer = this.options.tokenizer, this.tokenizer.options = this.options, this.tokenizer.lexer = this, this.inlineQueue = [], this.state = { inLink: !1, inRawBlock: !1, top: !0 };
    let n = { other: pe, block: et.normal, inline: je.normal };
    this.options.pedantic ? (n.block = et.pedantic, n.inline = je.pedantic) : this.options.gfm && (n.block = et.gfm, this.options.breaks ? n.inline = je.breaks : n.inline = je.gfm), this.tokenizer.rules = n;
  }
  static get rules() {
    return { block: et, inline: je };
  }
  static lex(t, n) {
    return new xt(n).lex(t);
  }
  static lexInline(t, n) {
    return new xt(n).inlineTokens(t);
  }
  lex(t) {
    t = t.replace(pe.carriageReturn, `
`), this.blockTokens(t, this.tokens);
    for (let n = 0; n < this.inlineQueue.length; n++) {
      let s = this.inlineQueue[n];
      this.inlineTokens(s.src, s.tokens);
    }
    return this.inlineQueue = [], this.tokens;
  }
  blockTokens(t, n = [], s = !1) {
    var o, l, i;
    this.tokenizer.lexer = this, this.options.pedantic && (t = t.replace(pe.tabCharGlobal, "    ").replace(pe.spaceLine, ""));
    let r = 1 / 0;
    for (; t; ) {
      if (t.length < r) r = t.length;
      else {
        this.infiniteLoopError(t.charCodeAt(0));
        break;
      }
      let a;
      if ((l = (o = this.options.extensions) == null ? void 0 : o.block) != null && l.some((c) => (a = c.call({ lexer: this }, t, n)) ? (t = t.substring(a.raw.length), n.push(a), !0) : !1)) continue;
      if (a = this.tokenizer.space(t)) {
        t = t.substring(a.raw.length);
        let c = n.at(-1);
        a.raw.length === 1 && c !== void 0 ? c.raw += `
` : n.push(a);
        continue;
      }
      if (a = this.tokenizer.code(t)) {
        t = t.substring(a.raw.length);
        let c = n.at(-1);
        (c == null ? void 0 : c.type) === "paragraph" || (c == null ? void 0 : c.type) === "text" ? (c.raw += (c.raw.endsWith(`
`) ? "" : `
`) + a.raw, c.text += `
` + a.text, this.inlineQueue.at(-1).src = c.text) : n.push(a);
        continue;
      }
      if (a = this.tokenizer.fences(t)) {
        t = t.substring(a.raw.length), n.push(a);
        continue;
      }
      if (a = this.tokenizer.heading(t)) {
        t = t.substring(a.raw.length), n.push(a);
        continue;
      }
      if (a = this.tokenizer.hr(t)) {
        t = t.substring(a.raw.length), n.push(a);
        continue;
      }
      if (a = this.tokenizer.blockquote(t)) {
        t = t.substring(a.raw.length), n.push(a);
        continue;
      }
      if (a = this.tokenizer.list(t)) {
        t = t.substring(a.raw.length), n.push(a);
        continue;
      }
      if (a = this.tokenizer.html(t)) {
        t = t.substring(a.raw.length), n.push(a);
        continue;
      }
      if (a = this.tokenizer.def(t)) {
        t = t.substring(a.raw.length);
        let c = n.at(-1);
        (c == null ? void 0 : c.type) === "paragraph" || (c == null ? void 0 : c.type) === "text" ? (c.raw += (c.raw.endsWith(`
`) ? "" : `
`) + a.raw, c.text += `
` + a.raw, this.inlineQueue.at(-1).src = c.text) : this.tokens.links[a.tag] || (this.tokens.links[a.tag] = { href: a.href, title: a.title }, n.push(a));
        continue;
      }
      if (a = this.tokenizer.table(t)) {
        t = t.substring(a.raw.length), n.push(a);
        continue;
      }
      if (a = this.tokenizer.lheading(t)) {
        t = t.substring(a.raw.length), n.push(a);
        continue;
      }
      let u = t;
      if ((i = this.options.extensions) != null && i.startBlock) {
        let c = 1 / 0, p = t.slice(1), d;
        this.options.extensions.startBlock.forEach((_) => {
          d = _.call({ lexer: this }, p), typeof d == "number" && d >= 0 && (c = Math.min(c, d));
        }), c < 1 / 0 && c >= 0 && (u = t.substring(0, c + 1));
      }
      if (this.state.top && (a = this.tokenizer.paragraph(u))) {
        let c = n.at(-1);
        s && (c == null ? void 0 : c.type) === "paragraph" ? (c.raw += (c.raw.endsWith(`
`) ? "" : `
`) + a.raw, c.text += `
` + a.text, this.inlineQueue.pop(), this.inlineQueue.at(-1).src = c.text) : n.push(a), s = u.length !== t.length, t = t.substring(a.raw.length);
        continue;
      }
      if (a = this.tokenizer.text(t)) {
        t = t.substring(a.raw.length);
        let c = n.at(-1);
        (c == null ? void 0 : c.type) === "text" ? (c.raw += (c.raw.endsWith(`
`) ? "" : `
`) + a.raw, c.text += `
` + a.text, this.inlineQueue.pop(), this.inlineQueue.at(-1).src = c.text) : n.push(a);
        continue;
      }
      if (t) {
        this.infiniteLoopError(t.charCodeAt(0));
        break;
      }
    }
    return this.state.top = !0, n;
  }
  inline(t, n = []) {
    return this.inlineQueue.push({ src: t, tokens: n }), n;
  }
  inlineTokens(t, n = []) {
    var u, c, p, d, _;
    this.tokenizer.lexer = this;
    let s = t, r = null;
    if (this.tokens.links) {
      let f = Object.keys(this.tokens.links);
      if (f.length > 0) for (; (r = this.tokenizer.rules.inline.reflinkSearch.exec(s)) !== null; ) f.includes(r[0].slice(r[0].lastIndexOf("[") + 1, -1)) && (s = s.slice(0, r.index) + "[" + "a".repeat(r[0].length - 2) + "]" + s.slice(this.tokenizer.rules.inline.reflinkSearch.lastIndex));
    }
    for (; (r = this.tokenizer.rules.inline.anyPunctuation.exec(s)) !== null; ) s = s.slice(0, r.index) + "++" + s.slice(this.tokenizer.rules.inline.anyPunctuation.lastIndex);
    let o;
    for (; (r = this.tokenizer.rules.inline.blockSkip.exec(s)) !== null; ) o = r[2] ? r[2].length : 0, s = s.slice(0, r.index + o) + "[" + "a".repeat(r[0].length - o - 2) + "]" + s.slice(this.tokenizer.rules.inline.blockSkip.lastIndex);
    s = ((c = (u = this.options.hooks) == null ? void 0 : u.emStrongMask) == null ? void 0 : c.call({ lexer: this }, s)) ?? s;
    let l = !1, i = "", a = 1 / 0;
    for (; t; ) {
      if (t.length < a) a = t.length;
      else {
        this.infiniteLoopError(t.charCodeAt(0));
        break;
      }
      l || (i = ""), l = !1;
      let f;
      if ((d = (p = this.options.extensions) == null ? void 0 : p.inline) != null && d.some(($) => (f = $.call({ lexer: this }, t, n)) ? (t = t.substring(f.raw.length), n.push(f), !0) : !1)) continue;
      if (f = this.tokenizer.escape(t)) {
        t = t.substring(f.raw.length), n.push(f);
        continue;
      }
      if (f = this.tokenizer.tag(t)) {
        t = t.substring(f.raw.length), n.push(f);
        continue;
      }
      if (f = this.tokenizer.link(t)) {
        t = t.substring(f.raw.length), n.push(f);
        continue;
      }
      if (f = this.tokenizer.reflink(t, this.tokens.links)) {
        t = t.substring(f.raw.length);
        let $ = n.at(-1);
        f.type === "text" && ($ == null ? void 0 : $.type) === "text" ? ($.raw += f.raw, $.text += f.text) : n.push(f);
        continue;
      }
      if (f = this.tokenizer.emStrong(t, s, i)) {
        t = t.substring(f.raw.length), n.push(f);
        continue;
      }
      if (f = this.tokenizer.codespan(t)) {
        t = t.substring(f.raw.length), n.push(f);
        continue;
      }
      if (f = this.tokenizer.br(t)) {
        t = t.substring(f.raw.length), n.push(f);
        continue;
      }
      if (f = this.tokenizer.del(t, s, i)) {
        t = t.substring(f.raw.length), n.push(f);
        continue;
      }
      if (f = this.tokenizer.autolink(t)) {
        t = t.substring(f.raw.length), n.push(f);
        continue;
      }
      if (!this.state.inLink && (f = this.tokenizer.url(t))) {
        t = t.substring(f.raw.length), n.push(f);
        continue;
      }
      let k = t;
      if ((_ = this.options.extensions) != null && _.startInline) {
        let $ = 1 / 0, I = t.slice(1), P;
        this.options.extensions.startInline.forEach((R) => {
          P = R.call({ lexer: this }, I), typeof P == "number" && P >= 0 && ($ = Math.min($, P));
        }), $ < 1 / 0 && $ >= 0 && (k = t.substring(0, $ + 1));
      }
      if (f = this.tokenizer.inlineText(k)) {
        t = t.substring(f.raw.length), f.raw.slice(-1) !== "_" && (i = f.raw.slice(-1)), l = !0;
        let $ = n.at(-1);
        ($ == null ? void 0 : $.type) === "text" ? ($.raw += f.raw, $.text += f.text) : n.push(f);
        continue;
      }
      if (t) {
        this.infiniteLoopError(t.charCodeAt(0));
        break;
      }
    }
    return n;
  }
  infiniteLoopError(t) {
    let n = "Infinite loop on byte: " + t;
    if (this.options.silent) console.error(n);
    else throw new Error(n);
  }
}, it = class {
  constructor(e) {
    G(this, "options");
    G(this, "parser");
    this.options = e || De;
  }
  space(e) {
    return "";
  }
  code({ text: e, lang: t, escaped: n }) {
    var o;
    let s = (o = (t || "").match(pe.notSpaceStart)) == null ? void 0 : o[0], r = e.replace(pe.endingNewline, "") + `
`;
    return s ? '<pre><code class="language-' + Ee(s) + '">' + (n ? r : Ee(r, !0)) + `</code></pre>
` : "<pre><code>" + (n ? r : Ee(r, !0)) + `</code></pre>
`;
  }
  blockquote({ tokens: e }) {
    return `<blockquote>
${this.parser.parse(e)}</blockquote>
`;
  }
  html({ text: e }) {
    return e;
  }
  def(e) {
    return "";
  }
  heading({ tokens: e, depth: t }) {
    return `<h${t}>${this.parser.parseInline(e)}</h${t}>
`;
  }
  hr(e) {
    return `<hr>
`;
  }
  list(e) {
    let t = e.ordered, n = e.start, s = "";
    for (let l = 0; l < e.items.length; l++) {
      let i = e.items[l];
      s += this.listitem(i);
    }
    let r = t ? "ol" : "ul", o = t && n !== 1 ? ' start="' + n + '"' : "";
    return "<" + r + o + `>
` + s + "</" + r + `>
`;
  }
  listitem(e) {
    return `<li>${this.parser.parse(e.tokens)}</li>
`;
  }
  checkbox({ checked: e }) {
    return "<input " + (e ? 'checked="" ' : "") + 'disabled="" type="checkbox"> ';
  }
  paragraph({ tokens: e }) {
    return `<p>${this.parser.parseInline(e)}</p>
`;
  }
  table(e) {
    let t = "", n = "";
    for (let r = 0; r < e.header.length; r++) n += this.tablecell(e.header[r]);
    t += this.tablerow({ text: n });
    let s = "";
    for (let r = 0; r < e.rows.length; r++) {
      let o = e.rows[r];
      n = "";
      for (let l = 0; l < o.length; l++) n += this.tablecell(o[l]);
      s += this.tablerow({ text: n });
    }
    return s && (s = `<tbody>${s}</tbody>`), `<table>
<thead>
` + t + `</thead>
` + s + `</table>
`;
  }
  tablerow({ text: e }) {
    return `<tr>
${e}</tr>
`;
  }
  tablecell(e) {
    let t = this.parser.parseInline(e.tokens), n = e.header ? "th" : "td";
    return (e.align ? `<${n} align="${e.align}">` : `<${n}>`) + t + `</${n}>
`;
  }
  strong({ tokens: e }) {
    return `<strong>${this.parser.parseInline(e)}</strong>`;
  }
  em({ tokens: e }) {
    return `<em>${this.parser.parseInline(e)}</em>`;
  }
  codespan({ text: e }) {
    return `<code>${Ee(e, !0)}</code>`;
  }
  br(e) {
    return "<br>";
  }
  del({ tokens: e }) {
    return `<del>${this.parser.parseInline(e)}</del>`;
  }
  link({ href: e, title: t, tokens: n }) {
    let s = this.parser.parseInline(n), r = fn(e);
    if (r === null) return s;
    e = r;
    let o = '<a href="' + e + '"';
    return t && (o += ' title="' + Ee(t) + '"'), o += ">" + s + "</a>", o;
  }
  image({ href: e, title: t, text: n, tokens: s }) {
    s && (n = this.parser.parseInline(s, this.parser.textRenderer));
    let r = fn(e);
    if (r === null) return Ee(n);
    e = r;
    let o = `<img src="${e}" alt="${Ee(n)}"`;
    return t && (o += ` title="${Ee(t)}"`), o += ">", o;
  }
  text(e) {
    return "tokens" in e && e.tokens ? this.parser.parseInline(e.tokens) : "escaped" in e && e.escaped ? e.text : Ee(e.text);
  }
}, Vt = class {
  strong({ text: e }) {
    return e;
  }
  em({ text: e }) {
    return e;
  }
  codespan({ text: e }) {
    return e;
  }
  del({ text: e }) {
    return e;
  }
  html({ text: e }) {
    return e;
  }
  text({ text: e }) {
    return e;
  }
  link({ text: e }) {
    return "" + e;
  }
  image({ text: e }) {
    return "" + e;
  }
  br() {
    return "";
  }
  checkbox({ raw: e }) {
    return e;
  }
}, ke = class Mt {
  constructor(t) {
    G(this, "options");
    G(this, "renderer");
    G(this, "textRenderer");
    this.options = t || De, this.options.renderer = this.options.renderer || new it(), this.renderer = this.options.renderer, this.renderer.options = this.options, this.renderer.parser = this, this.textRenderer = new Vt();
  }
  static parse(t, n) {
    return new Mt(n).parse(t);
  }
  static parseInline(t, n) {
    return new Mt(n).parseInline(t);
  }
  parse(t) {
    var s, r;
    this.renderer.parser = this;
    let n = "";
    for (let o = 0; o < t.length; o++) {
      let l = t[o];
      if ((r = (s = this.options.extensions) == null ? void 0 : s.renderers) != null && r[l.type]) {
        let a = l, u = this.options.extensions.renderers[a.type].call({ parser: this }, a);
        if (u !== !1 || !["space", "hr", "heading", "code", "table", "blockquote", "list", "html", "def", "paragraph", "text"].includes(a.type)) {
          n += u || "";
          continue;
        }
      }
      let i = l;
      switch (i.type) {
        case "space": {
          n += this.renderer.space(i);
          break;
        }
        case "hr": {
          n += this.renderer.hr(i);
          break;
        }
        case "heading": {
          n += this.renderer.heading(i);
          break;
        }
        case "code": {
          n += this.renderer.code(i);
          break;
        }
        case "table": {
          n += this.renderer.table(i);
          break;
        }
        case "blockquote": {
          n += this.renderer.blockquote(i);
          break;
        }
        case "list": {
          n += this.renderer.list(i);
          break;
        }
        case "checkbox": {
          n += this.renderer.checkbox(i);
          break;
        }
        case "html": {
          n += this.renderer.html(i);
          break;
        }
        case "def": {
          n += this.renderer.def(i);
          break;
        }
        case "paragraph": {
          n += this.renderer.paragraph(i);
          break;
        }
        case "text": {
          n += this.renderer.text(i);
          break;
        }
        default: {
          let a = 'Token with "' + i.type + '" type was not found.';
          if (this.options.silent) return console.error(a), "";
          throw new Error(a);
        }
      }
    }
    return n;
  }
  parseInline(t, n = this.renderer) {
    var r, o;
    this.renderer.parser = this;
    let s = "";
    for (let l = 0; l < t.length; l++) {
      let i = t[l];
      if ((o = (r = this.options.extensions) == null ? void 0 : r.renderers) != null && o[i.type]) {
        let u = this.options.extensions.renderers[i.type].call({ parser: this }, i);
        if (u !== !1 || !["escape", "html", "link", "image", "strong", "em", "codespan", "br", "del", "text"].includes(i.type)) {
          s += u || "";
          continue;
        }
      }
      let a = i;
      switch (a.type) {
        case "escape": {
          s += n.text(a);
          break;
        }
        case "html": {
          s += n.html(a);
          break;
        }
        case "link": {
          s += n.link(a);
          break;
        }
        case "image": {
          s += n.image(a);
          break;
        }
        case "checkbox": {
          s += n.checkbox(a);
          break;
        }
        case "strong": {
          s += n.strong(a);
          break;
        }
        case "em": {
          s += n.em(a);
          break;
        }
        case "codespan": {
          s += n.codespan(a);
          break;
        }
        case "br": {
          s += n.br(a);
          break;
        }
        case "del": {
          s += n.del(a);
          break;
        }
        case "text": {
          s += n.text(a);
          break;
        }
        default: {
          let u = 'Token with "' + a.type + '" type was not found.';
          if (this.options.silent) return console.error(u), "";
          throw new Error(u);
        }
      }
    }
    return s;
  }
}, tt, Fe = (tt = class {
  constructor(e) {
    G(this, "options");
    G(this, "block");
    this.options = e || De;
  }
  preprocess(e) {
    return e;
  }
  postprocess(e) {
    return e;
  }
  processAllTokens(e) {
    return e;
  }
  emStrongMask(e) {
    return e;
  }
  provideLexer(e = this.block) {
    return e ? me.lex : me.lexInline;
  }
  provideParser(e = this.block) {
    return e ? ke.parse : ke.parseInline;
  }
}, G(tt, "passThroughHooks", /* @__PURE__ */ new Set(["preprocess", "postprocess", "processAllTokens", "emStrongMask"])), G(tt, "passThroughHooksRespectAsync", /* @__PURE__ */ new Set(["preprocess", "postprocess", "processAllTokens"])), tt), ys = class {
  constructor(...e) {
    G(this, "defaults", Bt());
    G(this, "options", this.setOptions);
    G(this, "parse", this.parseMarkdown(!0));
    G(this, "parseInline", this.parseMarkdown(!1));
    G(this, "Parser", ke);
    G(this, "Renderer", it);
    G(this, "TextRenderer", Vt);
    G(this, "Lexer", me);
    G(this, "Tokenizer", ot);
    G(this, "Hooks", Fe);
    this.use(...e);
  }
  walkTokens(e, t) {
    var s, r;
    let n = [];
    for (let o of e) switch (n = n.concat(t.call(this, o)), o.type) {
      case "table": {
        let l = o;
        for (let i of l.header) n = n.concat(this.walkTokens(i.tokens, t));
        for (let i of l.rows) for (let a of i) n = n.concat(this.walkTokens(a.tokens, t));
        break;
      }
      case "list": {
        let l = o;
        n = n.concat(this.walkTokens(l.items, t));
        break;
      }
      default: {
        let l = o;
        (r = (s = this.defaults.extensions) == null ? void 0 : s.childTokens) != null && r[l.type] ? this.defaults.extensions.childTokens[l.type].forEach((i) => {
          let a = l[i].flat(1 / 0);
          n = n.concat(this.walkTokens(a, t));
        }) : l.tokens && (n = n.concat(this.walkTokens(l.tokens, t)));
      }
    }
    return n;
  }
  use(...e) {
    let t = this.defaults.extensions || { renderers: {}, childTokens: {} };
    return e.forEach((n) => {
      let s = { ...n };
      if (s.async = this.defaults.async || s.async || !1, n.extensions && (n.extensions.forEach((r) => {
        if (!r.name) throw new Error("extension name required");
        if ("renderer" in r) {
          let o = t.renderers[r.name];
          o ? t.renderers[r.name] = function(...l) {
            let i = r.renderer.apply(this, l);
            return i === !1 && (i = o.apply(this, l)), i;
          } : t.renderers[r.name] = r.renderer;
        }
        if ("tokenizer" in r) {
          if (!r.level || r.level !== "block" && r.level !== "inline") throw new Error("extension level must be 'block' or 'inline'");
          let o = t[r.level];
          o ? o.unshift(r.tokenizer) : t[r.level] = [r.tokenizer], r.start && (r.level === "block" ? t.startBlock ? t.startBlock.push(r.start) : t.startBlock = [r.start] : r.level === "inline" && (t.startInline ? t.startInline.push(r.start) : t.startInline = [r.start]));
        }
        "childTokens" in r && r.childTokens && (t.childTokens[r.name] = r.childTokens);
      }), s.extensions = t), n.renderer) {
        let r = this.defaults.renderer || new it(this.defaults);
        for (let o in n.renderer) {
          if (!(o in r)) throw new Error(`renderer '${o}' does not exist`);
          if (["options", "parser"].includes(o)) continue;
          let l = o, i = n.renderer[l], a = r[l];
          r[l] = (...u) => {
            let c = i.apply(r, u);
            return c === !1 && (c = a.apply(r, u)), c || "";
          };
        }
        s.renderer = r;
      }
      if (n.tokenizer) {
        let r = this.defaults.tokenizer || new ot(this.defaults);
        for (let o in n.tokenizer) {
          if (!(o in r)) throw new Error(`tokenizer '${o}' does not exist`);
          if (["options", "rules", "lexer"].includes(o)) continue;
          let l = o, i = n.tokenizer[l], a = r[l];
          r[l] = (...u) => {
            let c = i.apply(r, u);
            return c === !1 && (c = a.apply(r, u)), c;
          };
        }
        s.tokenizer = r;
      }
      if (n.hooks) {
        let r = this.defaults.hooks || new Fe();
        for (let o in n.hooks) {
          if (!(o in r)) throw new Error(`hook '${o}' does not exist`);
          if (["options", "block"].includes(o)) continue;
          let l = o, i = n.hooks[l], a = r[l];
          Fe.passThroughHooks.has(o) ? r[l] = (u) => {
            if (this.defaults.async && Fe.passThroughHooksRespectAsync.has(o)) return (async () => {
              let p = await i.call(r, u);
              return a.call(r, p);
            })();
            let c = i.call(r, u);
            return a.call(r, c);
          } : r[l] = (...u) => {
            if (this.defaults.async) return (async () => {
              let p = await i.apply(r, u);
              return p === !1 && (p = await a.apply(r, u)), p;
            })();
            let c = i.apply(r, u);
            return c === !1 && (c = a.apply(r, u)), c;
          };
        }
        s.hooks = r;
      }
      if (n.walkTokens) {
        let r = this.defaults.walkTokens, o = n.walkTokens;
        s.walkTokens = function(l) {
          let i = [];
          return i.push(o.call(this, l)), r && (i = i.concat(r.call(this, l))), i;
        };
      }
      this.defaults = { ...this.defaults, ...s };
    }), this;
  }
  setOptions(e) {
    return this.defaults = { ...this.defaults, ...e }, this;
  }
  lexer(e, t) {
    return me.lex(e, t ?? this.defaults);
  }
  parser(e, t) {
    return ke.parse(e, t ?? this.defaults);
  }
  parseMarkdown(e) {
    return (t, n) => {
      let s = { ...n }, r = { ...this.defaults, ...s }, o = this.onError(!!r.silent, !!r.async);
      if (this.defaults.async === !0 && s.async === !1) return o(new Error("marked(): The async option was set to true by an extension. Remove async: false from the parse options object to return a Promise."));
      if (typeof t > "u" || t === null) return o(new Error("marked(): input parameter is undefined or null"));
      if (typeof t != "string") return o(new Error("marked(): input parameter is of type " + Object.prototype.toString.call(t) + ", string expected"));
      if (r.hooks && (r.hooks.options = r, r.hooks.block = e), r.async) return (async () => {
        let l = r.hooks ? await r.hooks.preprocess(t) : t, i = await (r.hooks ? await r.hooks.provideLexer(e) : e ? me.lex : me.lexInline)(l, r), a = r.hooks ? await r.hooks.processAllTokens(i) : i;
        r.walkTokens && await Promise.all(this.walkTokens(a, r.walkTokens));
        let u = await (r.hooks ? await r.hooks.provideParser(e) : e ? ke.parse : ke.parseInline)(a, r);
        return r.hooks ? await r.hooks.postprocess(u) : u;
      })().catch(o);
      try {
        r.hooks && (t = r.hooks.preprocess(t));
        let l = (r.hooks ? r.hooks.provideLexer(e) : e ? me.lex : me.lexInline)(t, r);
        r.hooks && (l = r.hooks.processAllTokens(l)), r.walkTokens && this.walkTokens(l, r.walkTokens);
        let i = (r.hooks ? r.hooks.provideParser(e) : e ? ke.parse : ke.parseInline)(l, r);
        return r.hooks && (i = r.hooks.postprocess(i)), i;
      } catch (l) {
        return o(l);
      }
    };
  }
  onError(e, t) {
    return (n) => {
      if (n.message += `
Please report this to https://github.com/markedjs/marked.`, e) {
        let s = "<p>An error occurred:</p><pre>" + Ee(n.message + "", !0) + "</pre>";
        return t ? Promise.resolve(s) : s;
      }
      if (t) return Promise.reject(n);
      throw n;
    };
  }
}, Be = new ys();
function J(e, t) {
  return Be.parse(e, t);
}
J.options = J.setOptions = function(e) {
  return Be.setOptions(e), J.defaults = Be.defaults, os(J.defaults), J;
};
J.getDefaults = Bt;
J.defaults = De;
J.use = function(...e) {
  return Be.use(...e), J.defaults = Be.defaults, os(J.defaults), J;
};
J.walkTokens = function(e, t) {
  return Be.walkTokens(e, t);
};
J.parseInline = Be.parseInline;
J.Parser = ke;
J.parser = ke.parse;
J.Renderer = it;
J.TextRenderer = Vt;
J.Lexer = me;
J.lexer = me.lex;
J.Tokenizer = ot;
J.Hooks = Fe;
J.parse = J;
J.options;
J.setOptions;
J.use;
J.walkTokens;
J.parseInline;
ke.parse;
me.lex;
const wd = ["innerHTML"], rm = /* @__PURE__ */ z({
  __name: "Markdown",
  props: {
    content: {}
  },
  setup(e) {
    const t = e, n = new ys({ gfm: !0, breaks: !1 });
    n.use({
      renderer: {
        code(r) {
          const o = r.text, l = (r.lang || "").replace(/[^a-z0-9_-]/gi, ""), i = l && oe.getLanguage(l) ? oe.highlight(o, { language: l, ignoreIllegals: !0 }).value : oe.highlightAuto(o).value;
          return `<pre${l ? ` data-lang="${l}"` : ""}><code class="hljs">${i}</code></pre>`;
        }
      }
    });
    const s = Y(() => n.parse(t.content));
    return (r, o) => (g(), b("div", {
      class: "hive-md",
      innerHTML: s.value
    }, null, 8, wd));
  }
}), $d = /* @__PURE__ */ z({
  __name: "NewBadge",
  props: {
    variant: { default: "new" }
  },
  setup(e) {
    return (t, n) => (g(), b("span", {
      class: h([t.$style.badge, t.$style[e.variant]])
    }, [
      Q(t.$slots, "default", {}, () => [
        fe(x(e.variant), 1)
      ])
    ], 2));
  }
}), vd = "_badge_13w2z_2", Ed = "_draft_13w2z_21", Sd = "_beta_13w2z_26", Td = "_todo_13w2z_31", xd = {
  badge: vd,
  new: "_new_13w2z_16",
  draft: Ed,
  beta: Sd,
  todo: Td
}, Md = {
  $style: xd
}, mn = /* @__PURE__ */ D($d, [["__cssModules", Md]]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Ad = (e) => {
  for (const t in e)
    if (t.startsWith("aria-") || t === "role" || t === "title")
      return !0;
  return !1;
};
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const kn = (e) => e === "";
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Cd = (...e) => e.filter((t, n, s) => !!t && t.trim() !== "" && s.indexOf(t) === n).join(" ").trim();
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const _n = (e) => e.replace(/([a-z0-9])([A-Z])/g, "$1-$2").toLowerCase();
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Id = (e) => e.replace(
  /^([A-Z])|[\s-_]+(\w)/g,
  (t, n, s) => s ? s.toUpperCase() : n.toLowerCase()
);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Od = (e) => {
  const t = Id(e);
  return t.charAt(0).toUpperCase() + t.slice(1);
};
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
var He = {
  xmlns: "http://www.w3.org/2000/svg",
  width: 24,
  height: 24,
  viewBox: "0 0 24 24",
  fill: "none",
  stroke: "currentColor",
  "stroke-width": 2,
  "stroke-linecap": "round",
  "stroke-linejoin": "round"
};
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Nd = ({
  name: e,
  iconNode: t,
  absoluteStrokeWidth: n,
  "absolute-stroke-width": s,
  strokeWidth: r,
  "stroke-width": o,
  size: l = He.width,
  color: i = He.stroke,
  ...a
}, { slots: u }) => St(
  "svg",
  {
    ...He,
    ...a,
    width: l,
    height: l,
    stroke: i,
    "stroke-width": kn(n) || kn(s) || n === !0 || s === !0 ? Number(r || o || He["stroke-width"]) * 24 / Number(l) : r || o || He["stroke-width"],
    class: Cd(
      "lucide",
      a.class,
      ...e ? [`lucide-${_n(Od(e))}-icon`, `lucide-${_n(e)}`] : ["lucide-icon"]
    ),
    ...!u.default && !Ad(a) && { "aria-hidden": "true" }
  },
  [...t.map((c) => St(...c)), ...u.default ? [u.default()] : []]
);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const H = (e, t) => (n, { slots: s, attrs: r }) => St(
  Nd,
  {
    ...r,
    ...n,
    iconNode: t,
    name: e
  },
  s
);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Rd = H("arrow-right", [
  ["path", { d: "M5 12h14", key: "1ays0h" }],
  ["path", { d: "m12 5 7 7-7 7", key: "xquz4c" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Ld = H("bot", [
  ["path", { d: "M12 8V4H8", key: "hb8ula" }],
  ["rect", { width: "16", height: "12", x: "4", y: "8", rx: "2", key: "enze0r" }],
  ["path", { d: "M2 14h2", key: "vft8re" }],
  ["path", { d: "M20 14h2", key: "4cs60a" }],
  ["path", { d: "M15 13v2", key: "1xurst" }],
  ["path", { d: "M9 13v2", key: "rq6x2g" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Bd = H("brain", [
  ["path", { d: "M12 18V5", key: "adv99a" }],
  ["path", { d: "M15 13a4.17 4.17 0 0 1-3-4 4.17 4.17 0 0 1-3 4", key: "1e3is1" }],
  ["path", { d: "M17.598 6.5A3 3 0 1 0 12 5a3 3 0 1 0-5.598 1.5", key: "1gqd8o" }],
  ["path", { d: "M17.997 5.125a4 4 0 0 1 2.526 5.77", key: "iwvgf7" }],
  ["path", { d: "M18 18a4 4 0 0 0 2-7.464", key: "efp6ie" }],
  ["path", { d: "M19.967 17.483A4 4 0 1 1 12 18a4 4 0 1 1-7.967-.517", key: "1gq6am" }],
  ["path", { d: "M6 18a4 4 0 0 1-2-7.464", key: "k1g0md" }],
  ["path", { d: "M6.003 5.125a4 4 0 0 0-2.526 5.77", key: "q97ue3" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const zd = H("check", [["path", { d: "M20 6 9 17l-5-5", key: "1gmf2c" }]]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Dd = H("chevron-down", [
  ["path", { d: "m6 9 6 6 6-6", key: "qrunsl" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Pd = H("chevron-left", [
  ["path", { d: "m15 18-6-6 6-6", key: "1wnfg3" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Ud = H("chevron-right", [
  ["path", { d: "m9 18 6-6-6-6", key: "mthhwq" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const qd = H("chevron-up", [
  ["path", { d: "m18 15-6-6-6 6", key: "153udz" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const jd = H("circle-alert", [
  ["circle", { cx: "12", cy: "12", r: "10", key: "1mglay" }],
  ["line", { x1: "12", x2: "12", y1: "8", y2: "12", key: "1pkeuh" }],
  ["line", { x1: "12", x2: "12.01", y1: "16", y2: "16", key: "4dfq90" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Hd = H("circle-check", [
  ["circle", { cx: "12", cy: "12", r: "10", key: "1mglay" }],
  ["path", { d: "m9 12 2 2 4-4", key: "dzmm74" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Vd = H("circle-x", [
  ["circle", { cx: "12", cy: "12", r: "10", key: "1mglay" }],
  ["path", { d: "m15 9-6 6", key: "1uzhvr" }],
  ["path", { d: "m9 9 6 6", key: "z0biqf" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Fd = H("clock", [
  ["circle", { cx: "12", cy: "12", r: "10", key: "1mglay" }],
  ["path", { d: "M12 6v6l4 2", key: "mmk7yg" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Gd = H("copy", [
  ["rect", { width: "14", height: "14", x: "8", y: "8", rx: "2", ry: "2", key: "17jyea" }],
  ["path", { d: "M4 16c-1.1 0-2-.9-2-2V4c0-1.1.9-2 2-2h10c1.1 0 2 .9 2 2", key: "zix9uf" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Wd = H("cpu", [
  ["path", { d: "M12 20v2", key: "1lh1kg" }],
  ["path", { d: "M12 2v2", key: "tus03m" }],
  ["path", { d: "M17 20v2", key: "1rnc9c" }],
  ["path", { d: "M17 2v2", key: "11trls" }],
  ["path", { d: "M2 12h2", key: "1t8f8n" }],
  ["path", { d: "M2 17h2", key: "7oei6x" }],
  ["path", { d: "M2 7h2", key: "asdhe0" }],
  ["path", { d: "M20 12h2", key: "1q8mjw" }],
  ["path", { d: "M20 17h2", key: "1fpfkl" }],
  ["path", { d: "M20 7h2", key: "1o8tra" }],
  ["path", { d: "M7 20v2", key: "4gnj0m" }],
  ["path", { d: "M7 2v2", key: "1i4yhu" }],
  ["rect", { x: "4", y: "4", width: "16", height: "16", rx: "2", key: "1vbyd7" }],
  ["rect", { x: "8", y: "8", width: "8", height: "8", rx: "1", key: "z9xiuo" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Kd = H("database", [
  ["ellipse", { cx: "12", cy: "5", rx: "9", ry: "3", key: "msslwz" }],
  ["path", { d: "M3 5V19A9 3 0 0 0 21 19V5", key: "1wlel7" }],
  ["path", { d: "M3 12A9 3 0 0 0 21 12", key: "mv7ke4" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Zd = H("ellipsis", [
  ["circle", { cx: "12", cy: "12", r: "1", key: "41hilf" }],
  ["circle", { cx: "19", cy: "12", r: "1", key: "1wjl8i" }],
  ["circle", { cx: "5", cy: "12", r: "1", key: "1pcz8c" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Xd = H("external-link", [
  ["path", { d: "M15 3h6v6", key: "1q9fwt" }],
  ["path", { d: "M10 14 21 3", key: "gplh6r" }],
  ["path", { d: "M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6", key: "a6xqqp" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Yd = H("eye", [
  [
    "path",
    {
      d: "M2.062 12.348a1 1 0 0 1 0-.696 10.75 10.75 0 0 1 19.876 0 1 1 0 0 1 0 .696 10.75 10.75 0 0 1-19.876 0",
      key: "1nclc0"
    }
  ],
  ["circle", { cx: "12", cy: "12", r: "3", key: "1v7zrd" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Jd = H("file-text", [
  [
    "path",
    {
      d: "M6 22a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h8a2.4 2.4 0 0 1 1.704.706l3.588 3.588A2.4 2.4 0 0 1 20 8v12a2 2 0 0 1-2 2z",
      key: "1oefj6"
    }
  ],
  ["path", { d: "M14 2v5a1 1 0 0 0 1 1h5", key: "wfsgrz" }],
  ["path", { d: "M10 9H8", key: "b1mrlr" }],
  ["path", { d: "M16 13H8", key: "t4e002" }],
  ["path", { d: "M16 17H8", key: "z1uh3a" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Qd = H("funnel", [
  [
    "path",
    {
      d: "M10 20a1 1 0 0 0 .553.895l2 1A1 1 0 0 0 14 21v-7a2 2 0 0 1 .517-1.341L21.74 4.67A1 1 0 0 0 21 3H3a1 1 0 0 0-.742 1.67l7.225 7.989A2 2 0 0 1 10 14z",
      key: "sc7q7i"
    }
  ]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const eu = H("hash", [
  ["line", { x1: "4", x2: "20", y1: "9", y2: "9", key: "4lhtct" }],
  ["line", { x1: "4", x2: "20", y1: "15", y2: "15", key: "vyu0kd" }],
  ["line", { x1: "10", x2: "8", y1: "3", y2: "21", key: "1ggp8o" }],
  ["line", { x1: "16", x2: "14", y1: "3", y2: "21", key: "weycgp" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const tu = H("history", [
  ["path", { d: "M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8", key: "1357e3" }],
  ["path", { d: "M3 3v5h5", key: "1xhq8a" }],
  ["path", { d: "M12 7v5l4 2", key: "1fdv2h" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const nu = H("image", [
  ["rect", { width: "18", height: "18", x: "3", y: "3", rx: "2", ry: "2", key: "1m3agn" }],
  ["circle", { cx: "9", cy: "9", r: "2", key: "af1f0g" }],
  ["path", { d: "m21 15-3.086-3.086a2 2 0 0 0-2.828 0L6 21", key: "1xmnt7" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const su = H("info", [
  ["circle", { cx: "12", cy: "12", r: "10", key: "1mglay" }],
  ["path", { d: "M12 16v-4", key: "1dtifu" }],
  ["path", { d: "M12 8h.01", key: "e9boi3" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const ru = H("key-round", [
  [
    "path",
    {
      d: "M2.586 17.414A2 2 0 0 0 2 18.828V21a1 1 0 0 0 1 1h3a1 1 0 0 0 1-1v-1a1 1 0 0 1 1-1h1a1 1 0 0 0 1-1v-1a1 1 0 0 1 1-1h.172a2 2 0 0 0 1.414-.586l.814-.814a6.5 6.5 0 1 0-4-4z",
      key: "1s6t7t"
    }
  ],
  ["circle", { cx: "16.5", cy: "7.5", r: ".5", fill: "currentColor", key: "w0ekpg" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const au = H("loader-circle", [
  ["path", { d: "M21 12a9 9 0 1 1-6.219-8.56", key: "13zald" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const ou = H("menu", [
  ["path", { d: "M4 5h16", key: "1tepv9" }],
  ["path", { d: "M4 12h16", key: "1lakjw" }],
  ["path", { d: "M4 19h16", key: "1djgab" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const iu = H("message-square", [
  [
    "path",
    {
      d: "M22 17a2 2 0 0 1-2 2H6.828a2 2 0 0 0-1.414.586l-2.202 2.202A.71.71 0 0 1 2 21.286V5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2z",
      key: "18887p"
    }
  ]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const lu = H("network", [
  ["rect", { x: "16", y: "16", width: "6", height: "6", rx: "1", key: "4q2zg0" }],
  ["rect", { x: "2", y: "16", width: "6", height: "6", rx: "1", key: "8cvhb9" }],
  ["rect", { x: "9", y: "2", width: "6", height: "6", rx: "1", key: "1egb70" }],
  ["path", { d: "M5 16v-3a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v3", key: "1jsf9p" }],
  ["path", { d: "M12 12V8", key: "2874zd" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const cu = H("pause", [
  ["rect", { x: "14", y: "3", width: "5", height: "18", rx: "1", key: "kaeet6" }],
  ["rect", { x: "5", y: "3", width: "5", height: "18", rx: "1", key: "1wsw3u" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const du = H("pencil", [
  [
    "path",
    {
      d: "M21.174 6.812a1 1 0 0 0-3.986-3.987L3.842 16.174a2 2 0 0 0-.5.83l-1.321 4.352a.5.5 0 0 0 .623.622l4.353-1.32a2 2 0 0 0 .83-.497z",
      key: "1a8usu"
    }
  ],
  ["path", { d: "m15 5 4 4", key: "1mk7zo" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const uu = H("play", [
  [
    "path",
    {
      d: "M5 5a2 2 0 0 1 3.008-1.728l11.997 6.998a2 2 0 0 1 .003 3.458l-12 7A2 2 0 0 1 5 19z",
      key: "10ikf1"
    }
  ]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const hu = H("plus", [
  ["path", { d: "M5 12h14", key: "1ays0h" }],
  ["path", { d: "M12 5v14", key: "s699le" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const pu = H("refresh-cw", [
  ["path", { d: "M3 12a9 9 0 0 1 9-9 9.75 9.75 0 0 1 6.74 2.74L21 8", key: "v9h5vc" }],
  ["path", { d: "M21 3v5h-5", key: "1q7to0" }],
  ["path", { d: "M21 12a9 9 0 0 1-9 9 9.75 9.75 0 0 1-6.74-2.74L3 16", key: "3uifl3" }],
  ["path", { d: "M8 16H3v5", key: "1cv678" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const fu = H("search", [
  ["path", { d: "m21 21-4.34-4.34", key: "14j7rj" }],
  ["circle", { cx: "11", cy: "11", r: "8", key: "4ej97u" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const gu = H("send", [
  [
    "path",
    {
      d: "M14.536 21.686a.5.5 0 0 0 .937-.024l6.5-19a.496.496 0 0 0-.635-.635l-19 6.5a.5.5 0 0 0-.024.937l7.93 3.18a2 2 0 0 1 1.112 1.11z",
      key: "1ffxy3"
    }
  ],
  ["path", { d: "m21.854 2.147-10.94 10.939", key: "12cjpa" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const yu = H("settings", [
  [
    "path",
    {
      d: "M9.671 4.136a2.34 2.34 0 0 1 4.659 0 2.34 2.34 0 0 0 3.319 1.915 2.34 2.34 0 0 1 2.33 4.033 2.34 2.34 0 0 0 0 3.831 2.34 2.34 0 0 1-2.33 4.033 2.34 2.34 0 0 0-3.319 1.915 2.34 2.34 0 0 1-4.659 0 2.34 2.34 0 0 0-3.32-1.915 2.34 2.34 0 0 1-2.33-4.033 2.34 2.34 0 0 0 0-3.831A2.34 2.34 0 0 1 6.35 6.051a2.34 2.34 0 0 0 3.319-1.915",
      key: "1i5ecw"
    }
  ],
  ["circle", { cx: "12", cy: "12", r: "3", key: "1v7zrd" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const bu = H("share-2", [
  ["circle", { cx: "18", cy: "5", r: "3", key: "gq8acd" }],
  ["circle", { cx: "6", cy: "12", r: "3", key: "w7nqdw" }],
  ["circle", { cx: "18", cy: "19", r: "3", key: "1xt0gg" }],
  ["line", { x1: "8.59", x2: "15.42", y1: "13.51", y2: "17.49", key: "47mynk" }],
  ["line", { x1: "15.41", x2: "8.59", y1: "6.51", y2: "10.49", key: "1n3mei" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const mu = H("trash-2", [
  ["path", { d: "M10 11v6", key: "nco0om" }],
  ["path", { d: "M14 11v6", key: "outv1u" }],
  ["path", { d: "M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6", key: "miytrc" }],
  ["path", { d: "M3 6h18", key: "d0wm0j" }],
  ["path", { d: "M8 6V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2", key: "e791ji" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const ku = H("triangle-alert", [
  [
    "path",
    {
      d: "m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3",
      key: "wmoenq"
    }
  ],
  ["path", { d: "M12 9v4", key: "juzpu7" }],
  ["path", { d: "M12 17h.01", key: "p32p05" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const _u = H("wrench", [
  [
    "path",
    {
      d: "M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.106-3.105c.32-.322.863-.22.983.218a6 6 0 0 1-8.259 7.057l-7.91 7.91a1 1 0 0 1-2.999-3l7.91-7.91a6 6 0 0 1 7.057-8.259c.438.12.54.662.219.984z",
      key: "1ngwbx"
    }
  ]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const wu = H("x", [
  ["path", { d: "M18 6 6 18", key: "1bl5f8" }],
  ["path", { d: "m6 6 12 12", key: "d8bk6v" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const $u = H("zap", [
  [
    "path",
    {
      d: "M4 14a1 1 0 0 1-.78-1.63l9.9-10.2a.5.5 0 0 1 .86.46l-1.92 6.02A1 1 0 0 0 13 10h7a1 1 0 0 1 .78 1.63l-9.9 10.2a.5.5 0 0 1-.86-.46l1.92-6.02A1 1 0 0 0 11 14z",
      key: "1xq2db"
    }
  ]
]), bs = {
  // actions
  plus: hu,
  search: fu,
  close: wu,
  check: zd,
  edit: du,
  trash: mu,
  copy: Gd,
  refresh: pu,
  settings: yu,
  filter: Qd,
  more: Zd,
  menu: ou,
  send: gu,
  play: uu,
  pause: cu,
  external: Xd,
  // navigation
  "chevron-right": Ud,
  "chevron-down": Dd,
  "chevron-left": Pd,
  "chevron-up": qd,
  "arrow-right": Rd,
  // hive domain
  brain: Bd,
  database: Kd,
  wrench: _u,
  eye: Yd,
  image: nu,
  bolt: $u,
  network: lu,
  share: bu,
  key: ru,
  file: Jd,
  hash: eu,
  bot: Ld,
  cpu: Wd,
  chat: iu,
  clock: Fd,
  history: tu,
  // status
  ok: Hd,
  warn: ku,
  error: Vd,
  info: su,
  alert: jd,
  loader: au
}, am = Object.keys(bs), vu = /* @__PURE__ */ z({
  __name: "Icon",
  props: {
    name: {},
    size: { default: 16 },
    stroke: { default: 1.75 }
  },
  setup(e) {
    return (t, n) => (g(), ce(Js(nt(bs)[e.name]), {
      size: e.size,
      "stroke-width": e.stroke,
      class: h(t.$style.icon),
      "aria-hidden": "true"
    }, null, 8, ["size", "stroke-width", "class"]));
  }
}), Eu = "_icon_18v37_2", Su = {
  icon: Eu
}, Tu = {
  $style: Su
}, ms = /* @__PURE__ */ D(vu, [["__cssModules", Tu]]), xu = /* @__PURE__ */ z({
  __name: "Modal",
  props: {
    open: { type: Boolean },
    title: {},
    size: { default: "md" },
    closeOnBackdrop: { type: Boolean, default: !0 },
    closeOnEsc: { type: Boolean, default: !0 }
  },
  emits: ["close"],
  setup(e, { emit: t }) {
    const n = e, s = t;
    function r(o) {
      n.open && n.closeOnEsc && o.key === "Escape" && s("close");
    }
    return lt(() => n.open, (o) => {
      o ? window.addEventListener("keydown", r) : window.removeEventListener("keydown", r);
    }, { immediate: !0 }), Ct(() => window.removeEventListener("keydown", r)), (o, l) => (g(), ce(ct, { to: "body" }, [
      e.open ? (g(), b("div", {
        key: 0,
        class: h(o.$style.overlay),
        onClick: l[1] || (l[1] = On((i) => e.closeOnBackdrop && s("close"), ["self"]))
      }, [
        w("div", {
          class: h([o.$style.panel, o.$style[e.size]]),
          role: "dialog",
          "aria-modal": "true"
        }, [
          e.title || o.$slots.header ? (g(), b("header", {
            key: 0,
            class: h(o.$style.head)
          }, [
            Q(o.$slots, "header", {}, () => [
              w("span", {
                class: h(o.$style.title)
              }, x(e.title), 3)
            ]),
            w("button", {
              type: "button",
              class: h(o.$style.close),
              "aria-label": "Close",
              onClick: l[0] || (l[0] = (i) => s("close"))
            }, "×", 2)
          ], 2)) : L("", !0),
          w("div", {
            class: h(o.$style.body)
          }, [
            Q(o.$slots, "default")
          ], 2),
          o.$slots.footer ? (g(), b("footer", {
            key: 1,
            class: h(o.$style.foot)
          }, [
            Q(o.$slots, "footer")
          ], 2)) : L("", !0)
        ], 2)
      ], 2)) : L("", !0)
    ]));
  }
}), Mu = "_overlay_1yxgf_2", Au = "_panel_1yxgf_3", Cu = "_sm_1yxgf_4", Iu = "_md_1yxgf_4", Ou = "_lg_1yxgf_4", Nu = "_head_1yxgf_5", Ru = "_title_1yxgf_6", Lu = "_close_1yxgf_7", Bu = "_body_1yxgf_9", zu = "_foot_1yxgf_10", Du = {
  overlay: Mu,
  panel: Au,
  sm: Cu,
  md: Iu,
  lg: Ou,
  head: Nu,
  title: Ru,
  close: Lu,
  body: Bu,
  foot: zu
}, Pu = {
  $style: Du
}, Uu = /* @__PURE__ */ D(xu, [["__cssModules", Pu]]), qu = /* @__PURE__ */ z({
  __name: "ConfirmDialog",
  props: {
    open: { type: Boolean },
    title: { default: "Are you sure?" },
    message: {},
    confirmText: { default: "Confirm" },
    cancelText: { default: "Cancel" },
    danger: { type: Boolean, default: !1 }
  },
  emits: ["confirm", "cancel"],
  setup(e, { emit: t }) {
    const n = t;
    return (s, r) => (g(), ce(Uu, {
      open: e.open,
      title: e.title,
      size: "sm",
      onClose: r[2] || (r[2] = (o) => n("cancel"))
    }, {
      footer: be(() => [
        w("button", {
          type: "button",
          class: h(s.$style.cancel),
          onClick: r[0] || (r[0] = (o) => n("cancel"))
        }, x(e.cancelText), 3),
        w("button", {
          type: "button",
          class: h([s.$style.confirm, e.danger && s.$style.danger]),
          onClick: r[1] || (r[1] = (o) => n("confirm"))
        }, x(e.confirmText), 3)
      ]),
      default: be(() => [
        w("p", {
          class: h(s.$style.msg)
        }, [
          Q(s.$slots, "default", {}, () => [
            fe(x(e.message), 1)
          ])
        ], 2)
      ]),
      _: 3
    }, 8, ["open", "title"]));
  }
}), ju = "_msg_ad9ml_2", Hu = "_cancel_ad9ml_3", Vu = "_confirm_ad9ml_3", Fu = "_danger_ad9ml_7", Gu = {
  msg: ju,
  cancel: Hu,
  confirm: Vu,
  danger: Fu
}, Wu = {
  $style: Gu
}, om = /* @__PURE__ */ D(qu, [["__cssModules", Wu]]), Ku = /* @__PURE__ */ z({
  __name: "Drawer",
  props: {
    open: { type: Boolean },
    side: { default: "right" },
    title: {},
    width: { default: 420 }
  },
  emits: ["close"],
  setup(e, { emit: t }) {
    const n = e, s = t;
    function r(o) {
      n.open && o.key === "Escape" && s("close");
    }
    return lt(() => n.open, (o) => {
      o ? window.addEventListener("keydown", r) : window.removeEventListener("keydown", r);
    }, { immediate: !0 }), Ct(() => window.removeEventListener("keydown", r)), (o, l) => (g(), ce(ct, { to: "body" }, [
      e.open ? (g(), b("div", {
        key: 0,
        class: h(o.$style.overlay),
        onClick: l[1] || (l[1] = On((i) => s("close"), ["self"]))
      }, [
        w("aside", {
          class: h([o.$style.drawer, o.$style[e.side]]),
          style: Se({ width: e.width + "px" }),
          role: "dialog",
          "aria-modal": "true"
        }, [
          e.title || o.$slots.header ? (g(), b("header", {
            key: 0,
            class: h(o.$style.head)
          }, [
            Q(o.$slots, "header", {}, () => [
              w("span", {
                class: h(o.$style.title)
              }, x(e.title), 3)
            ]),
            w("button", {
              type: "button",
              class: h(o.$style.close),
              "aria-label": "Close",
              onClick: l[0] || (l[0] = (i) => s("close"))
            }, "×", 2)
          ], 2)) : L("", !0),
          w("div", {
            class: h(o.$style.body)
          }, [
            Q(o.$slots, "default")
          ], 2),
          o.$slots.footer ? (g(), b("footer", {
            key: 1,
            class: h(o.$style.foot)
          }, [
            Q(o.$slots, "footer")
          ], 2)) : L("", !0)
        ], 6)
      ], 2)) : L("", !0)
    ]));
  }
}), Zu = "_overlay_1r4yl_2", Xu = "_drawer_1r4yl_3", Yu = "_right_1r4yl_4", Ju = "_left_1r4yl_5", Qu = "_head_1r4yl_6", eh = "_title_1r4yl_7", th = "_close_1r4yl_8", nh = "_body_1r4yl_10", sh = "_foot_1r4yl_11", rh = {
  overlay: Zu,
  drawer: Xu,
  right: Yu,
  left: Ju,
  head: Qu,
  title: eh,
  close: th,
  body: nh,
  foot: sh
}, ah = {
  $style: rh
}, im = /* @__PURE__ */ D(Ku, [["__cssModules", ah]]), Ze = ae([]);
let oh = 0;
function Ve(e, t = "info", n = 4e3) {
  const s = ++oh;
  return Ze.value = [...Ze.value, { id: s, tone: t, message: e }], n > 0 && setTimeout(() => ks(s), n), s;
}
function ks(e) {
  Ze.value = Ze.value.filter((t) => t.id !== e);
}
function ih() {
  return {
    toasts: Ze,
    push: Ve,
    dismiss: ks,
    info: (e, t) => Ve(e, "info", t),
    success: (e, t) => Ve(e, "success", t),
    warn: (e, t) => Ve(e, "warn", t),
    error: (e, t) => Ve(e, "error", t ?? 6e3)
  };
}
const lh = ["onClick"], ch = /* @__PURE__ */ z({
  __name: "ToastHost",
  setup(e) {
    const { toasts: t, dismiss: n } = ih();
    return (s, r) => (g(), ce(ct, { to: "body" }, [
      w("div", {
        class: h(s.$style.host)
      }, [
        (g(!0), b(le, null, ye(nt(t), (o) => (g(), b("div", {
          key: o.id,
          class: h([s.$style.toast, s.$style[o.tone]]),
          role: "status",
          onClick: (l) => nt(n)(o.id)
        }, x(o.message), 11, lh))), 128))
      ], 2)
    ]));
  }
}), dh = "_host_45iih_2", uh = "_toast_45iih_3", hh = "_info_45iih_4", ph = "_success_45iih_5", fh = "_error_45iih_6", gh = "_warn_45iih_7", yh = {
  host: dh,
  toast: uh,
  info: hh,
  success: ph,
  error: fh,
  warn: gh
}, bh = {
  $style: yh
}, lm = /* @__PURE__ */ D(ch, [["__cssModules", bh]]), mh = ["value", "disabled"], kh = {
  key: 0,
  value: "",
  disabled: ""
}, _h = ["value", "disabled"], wh = /* @__PURE__ */ z({
  __name: "Select",
  props: {
    modelValue: {},
    options: {},
    placeholder: {},
    disabled: { type: Boolean, default: !1 },
    size: { default: "md" }
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const n = t;
    return (s, r) => (g(), b("div", {
      class: h([s.$style.wrap, s.$style[e.size]])
    }, [
      w("select", {
        class: h(s.$style.select),
        value: e.modelValue,
        disabled: e.disabled,
        onChange: r[0] || (r[0] = (o) => n("update:modelValue", o.target.value))
      }, [
        e.placeholder ? (g(), b("option", kh, x(e.placeholder), 1)) : L("", !0),
        (g(!0), b(le, null, ye(e.options, (o) => (g(), b("option", {
          key: o.value,
          value: o.value,
          disabled: o.disabled
        }, x(o.label), 9, _h))), 128))
      ], 42, mh),
      w("span", {
        class: h(s.$style.arrow)
      }, "▾", 2)
    ], 2));
  }
}), $h = "_wrap_32hoq_2", vh = "_select_32hoq_3", Eh = "_sm_32hoq_4", Sh = "_md_32hoq_5", Th = "_arrow_32hoq_8", xh = {
  wrap: $h,
  select: vh,
  sm: Eh,
  md: Sh,
  arrow: Th
}, Mh = {
  $style: xh
}, Ah = /* @__PURE__ */ D(wh, [["__cssModules", Mh]]), Ch = /* @__PURE__ */ z({
  __name: "ModelSelect",
  props: {
    modelValue: {},
    spine: {},
    connectionId: {},
    baseUrl: {},
    apiKey: {},
    placeholder: { default: "Select a model" },
    disabled: { type: Boolean, default: !1 },
    size: { default: "md" }
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const n = e, s = t, r = ae([]), o = ae(!1), l = ae(""), i = ae(!1);
    async function a() {
      if (!n.connectionId && !n.baseUrl) {
        r.value = [];
        return;
      }
      o.value = !0, l.value = "";
      try {
        r.value = n.connectionId ? await n.spine.listProviderModels(n.connectionId) : await n.spine.fetchModelIds(n.baseUrl, n.apiKey);
      } catch (d) {
        l.value = d.message || "provider unreachable", r.value = [];
      } finally {
        o.value = !1;
      }
    }
    Qs(a), lt(() => [n.connectionId, n.baseUrl, n.apiKey], a);
    const u = Y(() => {
      const d = r.value.map((_) => ({ label: _, value: _ }));
      return n.modelValue && !r.value.includes(n.modelValue) && d.unshift({ label: `${n.modelValue} (not in list)`, value: n.modelValue }), d;
    }), c = Y(() => i.value || !!l.value && r.value.length === 0), p = (d) => s("update:modelValue", d);
    return (d, _) => (g(), b("div", {
      class: h(d.$style.wrap)
    }, [
      c.value ? (g(), ce(oa, {
        key: 0,
        modelValue: e.modelValue,
        disabled: e.disabled,
        placeholder: "model id",
        "onUpdate:modelValue": p
      }, null, 8, ["modelValue", "disabled"])) : (g(), ce(Ah, {
        key: 1,
        modelValue: e.modelValue,
        options: u.value,
        placeholder: o.value ? "Loading models…" : e.placeholder,
        disabled: e.disabled || o.value,
        size: e.size,
        "onUpdate:modelValue": p
      }, null, 8, ["modelValue", "options", "placeholder", "disabled", "size"])),
      w("div", {
        class: h(d.$style.foot)
      }, [
        l.value ? (g(), b("span", {
          key: 0,
          class: h(d.$style.err)
        }, "⚠ " + x(l.value), 3)) : L("", !0),
        w("button", {
          type: "button",
          class: h(d.$style.toggle),
          onClick: _[0] || (_[0] = (f) => i.value = !i.value)
        }, x(c.value ? "pick from list" : "custom id"), 3)
      ], 2)
    ], 2));
  }
}), Ih = "_wrap_eqk69_2", Oh = "_foot_eqk69_3", Nh = "_err_eqk69_4", Rh = "_toggle_eqk69_5", Lh = {
  wrap: Ih,
  foot: Oh,
  err: Nh,
  toggle: Rh
}, Bh = {
  $style: Lh
}, cm = /* @__PURE__ */ D(Ch, [["__cssModules", Bh]]), zh = ["value", "placeholder"], Dh = /* @__PURE__ */ z({
  __name: "SearchField",
  props: {
    modelValue: {},
    placeholder: { default: "Search…" },
    size: { default: "md" }
  },
  emits: ["update:modelValue", "clear"],
  setup(e, { emit: t }) {
    const n = t;
    function s() {
      n("update:modelValue", ""), n("clear");
    }
    return (r, o) => (g(), b("div", {
      class: h([r.$style.wrap, r.$style[e.size]])
    }, [
      Ce(ms, {
        name: "search",
        size: 15,
        class: h(r.$style.icon)
      }, null, 8, ["class"]),
      w("input", {
        class: h(r.$style.input),
        type: "search",
        value: e.modelValue,
        placeholder: e.placeholder,
        onInput: o[0] || (o[0] = (l) => n("update:modelValue", l.target.value))
      }, null, 42, zh),
      e.modelValue ? (g(), b("button", {
        key: 0,
        type: "button",
        class: h(r.$style.clear),
        "aria-label": "Clear",
        onClick: s
      }, "×", 2)) : L("", !0)
    ], 2));
  }
}), Ph = "_wrap_1h6ub_2", Uh = "_icon_1h6ub_3", qh = "_input_1h6ub_4", jh = "_sm_1h6ub_5", Hh = "_md_1h6ub_6", Vh = "_clear_1h6ub_9", Fh = {
  wrap: Ph,
  icon: Uh,
  input: qh,
  sm: jh,
  md: Hh,
  clear: Vh
}, Gh = {
  $style: Fh
}, dm = /* @__PURE__ */ D(Dh, [["__cssModules", Gh]]), Wh = ["title"], Kh = { key: 0 }, Zh = /* @__PURE__ */ z({
  __name: "CopyButton",
  props: {
    value: {},
    label: {},
    size: { default: "sm" }
  },
  setup(e) {
    const t = e, n = ae(!1);
    async function s() {
      var r;
      try {
        await ((r = navigator.clipboard) == null ? void 0 : r.writeText(t.value)), n.value = !0, setTimeout(() => n.value = !1, 1500);
      } catch {
      }
    }
    return (r, o) => (g(), b("button", {
      type: "button",
      class: h([r.$style.btn, r.$style[e.size]]),
      title: e.label ?? "Copy",
      onClick: s
    }, [
      Ce(ms, {
        name: n.value ? "check" : "copy",
        size: 14
      }, null, 8, ["name"]),
      e.label || n.value ? (g(), b("span", Kh, x(n.value ? "Copied" : e.label), 1)) : L("", !0)
    ], 10, Wh));
  }
}), Xh = "_btn_1mypx_2", Yh = "_sm_1mypx_3", Jh = "_md_1mypx_4", Qh = {
  btn: Xh,
  sm: Yh,
  md: Jh
}, ep = {
  $style: Qh
}, um = /* @__PURE__ */ D(Zh, [["__cssModules", ep]]), tp = /* @__PURE__ */ z({
  __name: "StatusBadge",
  props: {
    status: {},
    label: {},
    map: {}
  },
  setup(e) {
    const t = e, n = {
      completed: "ok",
      done: "ok",
      success: "ok",
      succeeded: "ok",
      ok: "ok",
      enabled: "ok",
      active: "ok",
      live: "ok",
      online: "ok",
      completedwitherrors: "warn",
      warn: "warn",
      degraded: "warn",
      pending: "muted",
      queued: "muted",
      idle: "muted",
      disabled: "muted",
      inactive: "muted",
      offline: "muted",
      cancelled: "muted",
      running: "run",
      started: "run",
      busy: "run",
      failed: "err",
      error: "err",
      rejected: "err"
    }, s = Y(() => (t.map ?? n)[(t.status ?? "").toLowerCase()] ?? "muted");
    return (r, o) => (g(), b("span", {
      class: h(r.$style.wrap)
    }, [
      w("span", {
        class: h([r.$style.dot, r.$style[s.value]])
      }, null, 2),
      w("span", {
        class: h(r.$style.label)
      }, x(e.label ?? e.status), 3)
    ], 2));
  }
}), np = "_wrap_1jn39_2", sp = "_dot_1jn39_3", rp = "_ok_1jn39_4", ap = "_warn_1jn39_5", op = "_err_1jn39_6", ip = "_run_1jn39_7", lp = "_muted_1jn39_8", cp = "_label_1jn39_9", dp = {
  wrap: np,
  dot: sp,
  ok: rp,
  warn: ap,
  err: op,
  run: ip,
  muted: lp,
  label: cp
}, up = {
  $style: dp
}, hm = /* @__PURE__ */ D(tp, [["__cssModules", up]]), hp = { key: 0 }, pp = /* @__PURE__ */ z({
  __name: "JsonViewer",
  props: {
    value: {},
    name: {},
    depth: { default: 0 },
    defaultOpen: { type: Boolean, default: !0 }
  },
  setup(e) {
    const t = e, n = Y(() => t.value !== null && typeof t.value == "object"), s = Y(() => Array.isArray(t.value)), r = Y(() => n.value ? s.value ? t.value.map((u, c) => [String(c), u]) : Object.entries(t.value) : []), o = Y(() => s.value ? `[${r.value.length}]` : `{${r.value.length}}`), l = ae(t.defaultOpen && t.depth < 2);
    function i(u) {
      return u === null ? "null" : typeof u == "string" ? `"${u}"` : String(u);
    }
    function a(u) {
      return u === null ? "null" : typeof u == "string" ? "str" : typeof u == "number" ? "num" : typeof u == "boolean" ? "bool" : "other";
    }
    return (u, c) => {
      const p = er("JsonViewer", !0);
      return g(), b("div", {
        class: h(u.$style.node),
        style: Se({ paddingLeft: e.depth ? "12px" : "0" })
      }, [
        n.value ? (g(), b(le, { key: 0 }, [
          w("div", {
            class: h(u.$style.row),
            onClick: c[0] || (c[0] = (d) => l.value = !l.value)
          }, [
            w("span", {
              class: h(u.$style.chev)
            }, x(l.value ? "▾" : "▸"), 3),
            e.name !== void 0 ? (g(), b("span", {
              key: 0,
              class: h(u.$style.key)
            }, x(e.name) + ":", 3)) : L("", !0),
            w("span", {
              class: h(u.$style.summary)
            }, x(o.value), 3)
          ], 2),
          l.value ? (g(), b("div", hp, [
            (g(!0), b(le, null, ye(r.value, ([d, _]) => (g(), ce(p, {
              key: d,
              value: _,
              name: d,
              depth: e.depth + 1
            }, null, 8, ["value", "name", "depth"]))), 128))
          ])) : L("", !0)
        ], 64)) : (g(), b("div", {
          key: 1,
          class: h(u.$style.row)
        }, [
          e.name !== void 0 ? (g(), b("span", {
            key: 0,
            class: h(u.$style.key)
          }, x(e.name) + ":", 3)) : L("", !0),
          w("span", {
            class: h([u.$style.val, u.$style[a(e.value)]])
          }, x(i(e.value)), 3)
        ], 2))
      ], 6);
    };
  }
}), fp = "_node_bxy3l_2", gp = "_row_bxy3l_3", yp = "_chev_bxy3l_4", bp = "_summary_bxy3l_5", mp = "_key_bxy3l_6", kp = "_val_bxy3l_7", _p = "_str_bxy3l_8", wp = "_num_bxy3l_9", $p = "_bool_bxy3l_10", vp = "_other_bxy3l_12", Ep = {
  node: fp,
  row: gp,
  chev: yp,
  summary: bp,
  key: mp,
  val: kp,
  str: _p,
  num: wp,
  bool: $p,
  null: "_null_bxy3l_11",
  other: vp
}, Sp = {
  $style: Ep
}, pm = /* @__PURE__ */ D(pp, [["__cssModules", Sp]]), Tp = /* @__PURE__ */ z({
  __name: "Tag",
  props: {
    label: {},
    variant: { default: "soft" },
    removable: { type: Boolean, default: !1 }
  },
  emits: ["remove"],
  setup(e, { emit: t }) {
    const n = t;
    return (s, r) => (g(), b("span", {
      class: h([s.$style.tag, s.$style[e.variant]])
    }, [
      fe(x(e.label) + " ", 1),
      e.removable ? (g(), b("button", {
        key: 0,
        type: "button",
        class: h(s.$style.x),
        "aria-label": "Remove",
        onClick: r[0] || (r[0] = (o) => n("remove"))
      }, "×", 2)) : L("", !0)
    ], 2));
  }
}), xp = "_tag_jpp5m_2", Mp = "_soft_jpp5m_3", Ap = "_solid_jpp5m_4", Cp = "_x_jpp5m_5", Ip = {
  tag: xp,
  soft: Mp,
  solid: Ap,
  x: Cp
}, Op = {
  $style: Ip
}, Np = /* @__PURE__ */ D(Tp, [["__cssModules", Op]]), Rp = ["placeholder"], Lp = /* @__PURE__ */ z({
  __name: "TagInput",
  props: {
    modelValue: {},
    placeholder: { default: "+ tag" },
    lowercase: { type: Boolean, default: !1 }
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const n = e, s = t, r = ae("");
    function o() {
      let i = r.value.trim();
      n.lowercase && (i = i.toLowerCase()), i && !n.modelValue.includes(i) && s("update:modelValue", [...n.modelValue, i]), r.value = "";
    }
    function l(i) {
      s("update:modelValue", n.modelValue.filter((a) => a !== i));
    }
    return (i, a) => (g(), b("div", {
      class: h(i.$style.wrap)
    }, [
      (g(!0), b(le, null, ye(e.modelValue, (u) => (g(), ce(Np, {
        key: u,
        label: u,
        removable: "",
        onRemove: (c) => l(u)
      }, null, 8, ["label", "onRemove"]))), 128)),
      tr(w("input", {
        "onUpdate:modelValue": a[0] || (a[0] = (u) => r.value = u),
        class: h(i.$style.input),
        placeholder: e.placeholder,
        onKeyup: nr(o, ["enter"]),
        onBlur: o
      }, null, 42, Rp), [
        [sr, r.value]
      ])
    ], 2));
  }
}), Bp = "_wrap_o9yrm_2", zp = "_input_o9yrm_3", Dp = {
  wrap: Bp,
  input: zp
}, Pp = {
  $style: Dp
}, fm = /* @__PURE__ */ D(Lp, [["__cssModules", Pp]]), Up = /* @__PURE__ */ z({
  __name: "KeyValueList",
  props: {
    items: { default: () => [] }
  },
  setup(e) {
    return (t, n) => (g(), b("dl", {
      class: h(t.$style.list)
    }, [
      (g(!0), b(le, null, ye(e.items, (s, r) => (g(), b(le, { key: r }, [
        w("dt", {
          class: h(t.$style.k)
        }, x(s.label), 3),
        w("dd", {
          class: h(t.$style.v)
        }, x(s.value ?? "—"), 3)
      ], 64))), 128)),
      Q(t.$slots, "default")
    ], 2));
  }
}), qp = "_list_162yo_2", jp = "_k_162yo_3", Hp = "_v_162yo_4", Vp = {
  list: qp,
  k: jp,
  v: Hp
}, Fp = {
  $style: Vp
}, gm = /* @__PURE__ */ D(Up, [["__cssModules", Fp]]), Gp = /* @__PURE__ */ z({
  __name: "Spinner",
  props: {
    size: { default: 18 },
    stroke: { default: 2 }
  },
  setup(e) {
    return (t, n) => (g(), b("span", {
      class: h(t.$style.spinner),
      style: Se({ width: e.size + "px", height: e.size + "px", borderWidth: e.stroke + "px" }),
      role: "status",
      "aria-label": "Loading"
    }, null, 6));
  }
}), Wp = "_spinner_1v1r5_2", Kp = "_spin_1v1r5_2", Zp = {
  spinner: Wp,
  spin: Kp
}, Xp = {
  $style: Zp
}, Yp = /* @__PURE__ */ D(Gp, [["__cssModules", Xp]]), Jp = /* @__PURE__ */ z({
  __name: "LoadingState",
  props: {
    label: { default: "Loading…" }
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.wrap)
    }, [
      Ce(Yp, { size: 22 }),
      w("span", {
        class: h(t.$style.label)
      }, x(e.label), 3)
    ], 2));
  }
}), Qp = "_wrap_101xf_2", ef = "_label_101xf_3", tf = {
  wrap: Qp,
  label: ef
}, nf = {
  $style: tf
}, ym = /* @__PURE__ */ D(Jp, [["__cssModules", nf]]), sf = ["aria-selected", "onClick"], rf = /* @__PURE__ */ z({
  __name: "Tabs",
  props: {
    modelValue: {},
    tabs: {}
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const n = t;
    return (s, r) => (g(), b("div", null, [
      w("div", {
        class: h(s.$style.bar),
        role: "tablist"
      }, [
        (g(!0), b(le, null, ye(e.tabs, (o) => (g(), b("button", {
          key: o.value,
          type: "button",
          role: "tab",
          "aria-selected": e.modelValue === o.value,
          class: h([s.$style.tab, e.modelValue === o.value && s.$style.active]),
          onClick: (l) => n("update:modelValue", o.value)
        }, [
          fe(x(o.label), 1),
          o.badge !== void 0 ? (g(), b("span", {
            key: 0,
            class: h(s.$style.badge)
          }, x(o.badge), 3)) : L("", !0)
        ], 10, sf))), 128))
      ], 2),
      Q(s.$slots, "default", { active: e.modelValue })
    ]));
  }
}), af = "_bar_3rgvk_2", of = "_tab_3rgvk_3", lf = "_active_3rgvk_5", cf = "_badge_3rgvk_6", df = {
  bar: af,
  tab: of,
  active: lf,
  badge: cf
}, uf = {
  $style: df
}, bm = /* @__PURE__ */ D(rf, [["__cssModules", uf]]), hf = /* @__PURE__ */ z({
  __name: "Tooltip",
  props: {
    text: {},
    placement: { default: "top" }
  },
  setup(e) {
    return (t, n) => (g(), b("span", {
      class: h(t.$style.wrap),
      tabindex: "0"
    }, [
      Q(t.$slots, "default"),
      w("span", {
        class: h([t.$style.bubble, t.$style[e.placement]]),
        role: "tooltip"
      }, x(e.text), 3)
    ], 2));
  }
}), pf = "_wrap_1y5fd_2", ff = "_bubble_1y5fd_3", gf = "_top_1y5fd_4", yf = "_bottom_1y5fd_5", bf = {
  wrap: pf,
  bubble: ff,
  top: gf,
  bottom: yf
}, mf = {
  $style: bf
}, mm = /* @__PURE__ */ D(hf, [["__cssModules", mf]]), kf = /* @__PURE__ */ z({
  __name: "Collapsible",
  props: {
    open: { type: Boolean },
    title: {},
    defaultOpen: { type: Boolean, default: !1 }
  },
  emits: ["update:open"],
  setup(e, { emit: t }) {
    const n = e, s = t, r = ae(n.open ?? n.defaultOpen);
    lt(() => n.open, (l) => {
      l !== void 0 && (r.value = l);
    });
    function o() {
      r.value = !r.value, s("update:open", r.value);
    }
    return (l, i) => (g(), b("div", {
      class: h(l.$style.wrap)
    }, [
      w("button", {
        type: "button",
        class: h(l.$style.head),
        onClick: o
      }, [
        w("span", {
          class: h(l.$style.chev)
        }, x(r.value ? "▾" : "▸"), 3),
        Q(l.$slots, "header", {}, () => [
          w("span", {
            class: h(l.$style.title)
          }, x(e.title), 3)
        ])
      ], 2),
      r.value ? (g(), b("div", {
        key: 0,
        class: h(l.$style.body)
      }, [
        Q(l.$slots, "default")
      ], 2)) : L("", !0)
    ], 2));
  }
}), _f = "_wrap_a99st_2", wf = "_head_a99st_3", $f = "_chev_a99st_5", vf = "_title_a99st_6", Ef = "_body_a99st_7", Sf = {
  wrap: _f,
  head: wf,
  chev: $f,
  title: vf,
  body: Ef
}, Tf = {
  $style: Sf
}, km = /* @__PURE__ */ D(kf, [["__cssModules", Tf]]), xf = /* @__PURE__ */ z({
  __name: "FlowNode",
  props: {
    node: {},
    selected: { type: Boolean, default: !1 },
    orphan: { type: Boolean, default: !1 },
    draggable: { type: Boolean, default: !1 },
    dragging: { type: Boolean, default: !1 },
    inCount: { default: 0 },
    outCount: { default: 0 }
  },
  emits: ["inspect", "pointer-down"],
  setup(e, { emit: t }) {
    const n = e, s = t, r = Y(() => ({
      trigger_chat: "trigger",
      trigger_webhook: "trigger",
      agent: "agent",
      ai: "agent",
      output_chat: "output",
      memory_read: "memory",
      memory_write: "memory",
      if_else: "if/else"
    })[n.node.nodeType] ?? n.node.nodeType), o = ["memory_read", "memory_write", "if_else"], l = Y(() => o.includes(n.node.nodeType)), i = Y(() => {
      if (n.node.nodeType !== "ai" && n.node.nodeType !== "agent") return null;
      const d = n.node.config, _ = d.model ?? "", f = _.indexOf("/");
      return {
        provider: f > 0 ? _.slice(0, f) : null,
        model: f > 0 ? _.slice(f + 1) : _ || null,
        systemPrompt: d.system_prompt ?? null,
        toolCount: (d.tools ?? []).length
      };
    }), a = Y(() => {
      var d;
      return n.node.description ? u(n.node.description, 180) : (d = i.value) != null && d.systemPrompt ? u(i.value.systemPrompt, 140) : null;
    });
    function u(d, _) {
      return d.length <= _ ? d : d.slice(0, _).trimEnd() + "…";
    }
    function c(d) {
      !n.draggable || d.target.closest("[data-no-drag]") || s("pointer-down", d, n.node);
    }
    function p(d) {
      d.stopPropagation(), s("inspect", n.node);
    }
    return (d, _) => (g(), b("div", {
      class: h([
        d.$style.node,
        e.selected && d.$style.selected,
        e.orphan && d.$style.orphan,
        e.draggable && d.$style.draggable,
        e.dragging && d.$style.dragging
      ]),
      onPointerdown: c
    }, [
      Ce(fo, null, {
        head: be(() => [
          Ce(Eo, { variant: "live" }),
          w("span", {
            class: h(d.$style.name)
          }, x(e.node.label), 3),
          w("span", {
            class: h(d.$style.spacer)
          }, null, 2),
          l.value ? (g(), ce(mn, {
            key: 0,
            variant: "todo"
          }, {
            default: be(() => [..._[0] || (_[0] = [
              fe("deprecated", -1)
            ])]),
            _: 1
          })) : L("", !0),
          Ce(Nn, { variant: "inactive" }, {
            default: be(() => [
              fe(x(r.value), 1)
            ]),
            _: 1
          }),
          w("button", {
            type: "button",
            "data-no-drag": "",
            class: h(d.$style.inspectBtn),
            "aria-label": "Edit node",
            title: "Edit node",
            onClick: p
          }, "✎", 2)
        ]),
        foot: be(() => {
          var f, k, $;
          return [
            w("div", {
              class: h(d.$style.statGrid)
            }, [
              w("div", {
                class: h(d.$style.stat)
              }, [
                w("span", {
                  class: h(d.$style.statLabel)
                }, "PROVIDER", 2),
                w("span", {
                  class: h(d.$style.statValue)
                }, x(((f = i.value) == null ? void 0 : f.provider) ?? "—"), 3)
              ], 2),
              w("div", {
                class: h(d.$style.stat)
              }, [
                w("span", {
                  class: h(d.$style.statLabel)
                }, "MODEL", 2),
                w("span", {
                  class: h(d.$style.statValue)
                }, x(((k = i.value) == null ? void 0 : k.model) ?? "—"), 3)
              ], 2),
              w("div", {
                class: h(d.$style.stat)
              }, [
                w("span", {
                  class: h(d.$style.statLabel)
                }, "TOOLS", 2),
                w("span", {
                  class: h(d.$style.statValue)
                }, x((($ = i.value) == null ? void 0 : $.toolCount) ?? 0), 3)
              ], 2),
              w("div", {
                class: h([d.$style.stat, d.$style.statSplit])
              }, [
                w("div", {
                  class: h(d.$style.splitHalf)
                }, [
                  w("span", {
                    class: h(d.$style.statLabel)
                  }, "PORTS", 2),
                  w("span", {
                    class: h(d.$style.statValue)
                  }, x(e.inCount), 3)
                ], 2),
                w("div", {
                  class: h(d.$style.splitHalf)
                }, [
                  w("span", {
                    class: h(d.$style.statLabel)
                  }, "EXITS", 2),
                  w("span", {
                    class: h(d.$style.statValue)
                  }, x(e.outCount), 3)
                ], 2)
              ], 2)
            ], 2)
          ];
        }),
        default: be(() => [
          a.value ? (g(), b("p", {
            key: 0,
            class: h(d.$style.description)
          }, x(a.value), 3)) : (g(), b("p", {
            key: 1,
            class: h(d.$style.descriptionMissing)
          }, [
            Ce(mn, { variant: "todo" }, {
              default: be(() => [..._[1] || (_[1] = [
                fe("todo", -1)
              ])]),
              _: 1
            }),
            w("span", {
              class: h(d.$style.commentPrefix)
            }, "// description", 2)
          ], 2))
        ]),
        _: 1
      })
    ], 34));
  }
}), Mf = "_node_1ke6m_2", Af = "_draggable_1ke6m_12", Cf = "_dragging_1ke6m_15", If = "_selected_1ke6m_19", Of = "_orphan_1ke6m_23", Nf = "_name_1ke6m_32", Rf = "_spacer_1ke6m_43", Lf = "_inspectBtn_1ke6m_45", Bf = "_description_1ke6m_64", zf = "_descriptionMissing_1ke6m_74", Df = "_commentPrefix_1ke6m_76", Pf = "_statGrid_1ke6m_82", Uf = "_stat_1ke6m_82", qf = "_statLabel_1ke6m_101", jf = "_statValue_1ke6m_109", Hf = "_statSplit_1ke6m_118", Vf = "_splitHalf_1ke6m_120", Ff = {
  node: Mf,
  draggable: Af,
  dragging: Cf,
  selected: If,
  orphan: Of,
  name: Nf,
  spacer: Rf,
  inspectBtn: Lf,
  description: Bf,
  descriptionMissing: zf,
  commentPrefix: Df,
  statGrid: Pf,
  stat: Uf,
  statLabel: qf,
  statValue: jf,
  statSplit: Hf,
  splitHalf: Vf
}, Gf = {
  $style: Ff
}, Wf = /* @__PURE__ */ D(xf, [["__cssModules", Gf]]), Kf = /* @__PURE__ */ z({
  __name: "FlowConnector",
  props: {
    label: {}
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h([t.$style.connector, e.label && t.$style.labeled])
    }, [
      e.label ? (g(), b("span", {
        key: 0,
        class: h(t.$style.label)
      }, x(e.label), 3)) : L("", !0),
      w("div", {
        class: h(t.$style.line)
      }, null, 2),
      w("span", {
        class: h(t.$style.arrow)
      }, null, 2)
    ], 2));
  }
}), Zf = "_connector_1ewyl_2", Xf = "_labeled_1ewyl_13", Yf = "_line_1ewyl_18", Jf = "_arrow_1ewyl_28", Qf = "_label_1ewyl_13", eg = {
  connector: Zf,
  labeled: Xf,
  line: Yf,
  arrow: Jf,
  label: Qf
}, tg = {
  $style: eg
}, wn = /* @__PURE__ */ D(Kf, [["__cssModules", tg]]);
function ng(e) {
  const t = new Map(e.nodes.map((a) => [a.id, a])), n = /* @__PURE__ */ new Map();
  for (const a of e.controlEdges) {
    const u = n.get(a.fromNodeId);
    u ? u.push(a) : n.set(a.fromNodeId, [a]);
  }
  const s = new Set(e.controlEdges.map((a) => a.toNodeId)), r = e.nodes.find((a) => !s.has(a.id)), o = [], l = /* @__PURE__ */ new Set();
  let i = r;
  for (; i && !l.has(i.id); ) {
    l.add(i.id);
    const a = n.get(i.id) ?? [];
    if (o.push({
      kind: "node",
      key: i.id,
      node: i,
      branchCount: a.length > 1 ? a.length - 1 : 0
    }), a.length === 0) break;
    const u = a[0];
    o.push({ kind: "edge", key: u.id, port: u.fromPort }), i = t.get(u.toNodeId);
  }
  for (const a of e.nodes)
    l.has(a.id) || o.push({ kind: "node", key: a.id, node: a, orphan: !0 });
  return o;
}
const At = 5, Le = ae(null), Ge = ae(null);
let Ft = null, Gt = null;
function _s(e) {
  Le.value && (Le.value = {
    ...Le.value,
    cursorX: e.clientX,
    cursorY: e.clientY
  });
}
function ws() {
  const e = Ft, t = Ge.value;
  vs(), e == null || e(t);
}
function $s(e) {
  if (e.key === "Escape") {
    const t = Gt;
    vs(), t == null || t();
  }
}
function vs() {
  window.removeEventListener("pointermove", _s), window.removeEventListener("pointerup", ws), window.removeEventListener("keydown", $s), Le.value = null, Ge.value = null, Ft = null, Gt = null;
}
function sg() {
  return {
    drag: Le,
    isDragging: Y(() => Le.value !== null),
    dropIndex: Ge,
    beginDrag(e, t) {
      const n = t.sourceRect;
      Le.value = {
        source: e,
        cursorX: t.cursorX,
        cursorY: t.cursorY,
        offsetX: n ? t.cursorX - n.left : 0,
        offsetY: n ? t.cursorY - n.top : 0
      }, Ge.value = null, Ft = t.onDrop ?? null, Gt = t.onCancel ?? null, window.addEventListener("pointermove", _s, { passive: !0 }), window.addEventListener("pointerup", ws, { passive: !0 }), window.addEventListener("keydown", $s);
    },
    setDropIndex(e) {
      Ge.value = e;
    },
    THRESHOLD_PX: At
  };
}
function Es(e, t, n, s) {
  const r = e - n, o = t - s;
  return r * r + o * o >= At * At;
}
const rg = {
  key: 3,
  style: { "font-family": "var(--font-mono)", "font-size": "13px", color: "var(--color-muted)", "align-self": "center", padding: "0 16px", "white-space": "nowrap" }
}, _t = "__phantom__", ag = /* @__PURE__ */ z({
  __name: "FlowCanvas",
  props: {
    flow: {},
    selectedNodeId: {},
    reorderable: { type: Boolean, default: !1 },
    droppable: { type: Boolean, default: !1 }
  },
  emits: ["select", "drop"],
  setup(e, { expose: t, emit: n }) {
    const s = e, r = n, o = Y(() => ng(s.flow)), l = Y(
      () => o.value.filter((E) => E.kind === "node").map((E) => E.kind === "node" ? E.node.id : "")
    ), i = Y(() => new Map(s.flow.nodes.map((E) => [E.id, E]))), a = Y(() => {
      const E = /* @__PURE__ */ new Map(), C = /* @__PURE__ */ new Map();
      for (const O of s.flow.controlEdges)
        E.set(O.toNodeId, (E.get(O.toNodeId) ?? 0) + 1), C.set(O.fromNodeId, (C.get(O.fromNodeId) ?? 0) + 1);
      return { inCount: E, outCount: C };
    });
    Y(() => s.reorderable || s.droppable);
    const u = sg(), c = ae(null), p = ae(null);
    let d = null;
    const _ = ae(null), f = Y(() => {
      const E = l.value, C = p.value, O = u.dropIndex.value;
      if (!C || O === null) return E;
      if (C.kind === "node") {
        const y = E.indexOf(C.nodeId);
        if (y === -1) return E;
        const m = E.filter((v) => v !== C.nodeId), S = y < O ? O - 1 : O;
        return [...m.slice(0, S), C.nodeId, ...m.slice(S)];
      }
      return C.kind === "palette" ? [...E.slice(0, O), _t, ...E.slice(O)] : E;
    }), k = Y(() => {
      var y;
      const E = f.value;
      if (E.length === 0) return [];
      const C = [];
      for (let m = 0; m < E.length; m++) {
        const S = E[m];
        if (S === _t) {
          const v = p.value, U = (v == null ? void 0 : v.kind) === "palette" ? v.item : { id: "x", nodeType: "unknown", label: "New node" };
          C.push({ kind: "phantom", key: _t, phantom: U });
        } else {
          const v = i.value.get(S);
          if (!v) continue;
          const U = ((y = p.value) == null ? void 0 : y.kind) === "node" && p.value.nodeId === S;
          C.push({
            kind: "node",
            key: S,
            node: {
              id: v.id,
              nodeType: v.nodeType,
              label: v.label,
              description: v.description,
              config: v.config
            },
            isDragging: U
          });
        }
        m < E.length - 1 && C.push({ kind: "edge", key: `edge-${m}` });
      }
      const O = new Set(E);
      for (const m of s.flow.nodes)
        O.has(m.id) || (C.length > 0 && C.push({ kind: "edge", key: `edge-orphan-${m.id}` }), C.push({
          kind: "node",
          key: m.id,
          node: {
            id: m.id,
            nodeType: m.nodeType,
            label: m.label,
            description: m.description,
            config: m.config
          },
          orphan: !0
        }));
      return C;
    }), $ = Y(() => s.flow.nodes.length === 0);
    function I(E, C) {
      if (!s.reorderable) return;
      const O = E.currentTarget ?? E.target.closest("[data-node-slot]");
      O && (d = {
        source: { kind: "node", nodeId: C.id },
        originX: E.clientX,
        originY: E.clientY,
        rect: O.getBoundingClientRect(),
        nodeEl: O
      }, window.addEventListener("pointermove", P), window.addEventListener("pointerup", R), window.addEventListener("pointercancel", R));
    }
    function P(E) {
      d && Es(E.clientX, E.clientY, d.originX, d.originY) && (M(d.source, E.clientX, E.clientY, d.rect), B());
    }
    function R() {
      const E = d == null ? void 0 : d.source;
      if (B(), (E == null ? void 0 : E.kind) === "node") {
        const C = i.value.get(E.nodeId);
        C && r("select", C);
      }
    }
    function B() {
      d = null, window.removeEventListener("pointermove", P), window.removeEventListener("pointerup", R), window.removeEventListener("pointercancel", R);
    }
    function F(E, C, O) {
      M({ kind: "palette", item: E }, C.clientX, C.clientY, O);
    }
    function M(E, C, O, y) {
      p.value = E;
      const m = (y == null ? void 0 : y.width) ?? 280, S = (y == null ? void 0 : y.height) ?? 180, v = y ? C - y.left : m / 2, U = y ? O - y.top : S / 2;
      c.value = { x: C - v, y: O - U, w: m, h: S, payload: E }, u.beginDrag(E, {
        cursorX: C,
        cursorY: O,
        sourceRect: y,
        onDrop: te,
        onCancel: de
      }), window.addEventListener("pointermove", q, { passive: !0 });
    }
    function q(E) {
      var y, m;
      if (!c.value) return;
      const C = ((y = u.drag.value) == null ? void 0 : y.offsetX) ?? c.value.w / 2, O = ((m = u.drag.value) == null ? void 0 : m.offsetY) ?? c.value.h / 2;
      c.value = { ...c.value, x: E.clientX - C, y: E.clientY - O }, u.setDropIndex(T(E.clientX));
    }
    function T(E) {
      const C = _.value;
      if (!C) return null;
      const O = Array.from(C.querySelectorAll("[data-node-slot]"));
      if (O.length === 0) return 0;
      let y = 0;
      for (const m of O) {
        const S = m.getBoundingClientRect();
        if (E > S.left + S.width / 2) y++;
        else break;
      }
      return y;
    }
    function te(E) {
      const C = p.value;
      if (C && E !== null) {
        const O = re(E, C);
        O && r("drop", { source: C, target: O });
      }
      se();
    }
    function re(E, C) {
      const O = l.value, y = C.kind === "node" ? O.filter((v) => v !== C.nodeId) : O;
      if (C.kind === "node") {
        const v = O.indexOf(C.nodeId);
        if ((v < E ? E - 1 : E) === v) return null;
      }
      const m = y[E] ?? null;
      return { kind: "between-nodes", afterNodeId: y[E - 1] ?? null, beforeNodeId: m };
    }
    function de() {
      se();
    }
    function se() {
      p.value = null, c.value = null, window.removeEventListener("pointermove", q);
    }
    return Ct(() => {
      B(), se();
    }), t({ startPaletteDrag: F }), (E, C) => {
      var O, y;
      return g(), b("div", {
        class: h(E.$style.scroll)
      }, [
        w("div", {
          class: h(E.$style.inner),
          ref_key: "slotsEl",
          ref: _
        }, [
          E.$slots.lead ? (g(), b("div", {
            key: 0,
            class: h(E.$style.slot)
          }, [
            Q(E.$slots, "lead")
          ], 2)) : L("", !0),
          E.$slots.lead && !$.value ? (g(), ce(wn, { key: 1 })) : L("", !0),
          $.value && !E.$slots.lead ? (g(), ce(bi, {
            key: 2,
            title: "Empty flow",
            sub: "Drag a node from the palette to get started."
          })) : $.value ? (g(), b("span", rg, "// add an Agent node from the palette →")) : (g(!0), b(le, { key: 4 }, ye(k.value, (m) => {
            var S, v;
            return g(), b(le, {
              key: m.key
            }, [
              m.kind === "node" ? (g(), b("div", {
                key: 0,
                "data-node-slot": "",
                class: h(E.$style.slot)
              }, [
                Ce(Wf, {
                  node: m.node,
                  selected: m.node.id === e.selectedNodeId,
                  orphan: m.orphan,
                  dragging: m.isDragging,
                  draggable: e.reorderable && !m.orphan,
                  "in-count": a.value.inCount.get(m.node.id) ?? 0,
                  "out-count": a.value.outCount.get(m.node.id) ?? 0,
                  onPointerDown: I,
                  onInspect: C[0] || (C[0] = (U) => r("select", U))
                }, null, 8, ["node", "selected", "orphan", "dragging", "draggable", "in-count", "out-count"])
              ], 2)) : m.kind === "phantom" ? (g(), b("div", {
                key: 1,
                "data-node-slot": "",
                class: h([E.$style.slot, E.$style.phantom])
              }, [
                w("div", {
                  class: h(E.$style.phantomCard)
                }, [
                  w("span", {
                    class: h(E.$style.phantomLabel)
                  }, x(((S = m.phantom) == null ? void 0 : S.label) ?? "New node"), 3),
                  w("span", {
                    class: h(E.$style.phantomType)
                  }, x(((v = m.phantom) == null ? void 0 : v.nodeType) ?? ""), 3)
                ], 2)
              ], 2)) : (g(), ce(wn, { key: 2 }))
            ], 64);
          }), 128))
        ], 2),
        (g(), ce(ct, { to: "body" }, [
          c.value ? (g(), b("div", {
            key: 0,
            class: h(E.$style.ghost),
            style: Se({
              transform: `translate(${c.value.x}px, ${c.value.y}px)`,
              width: c.value.w + "px"
            })
          }, [
            w("div", {
              class: h(E.$style.ghostInner)
            }, [
              c.value.payload.kind === "node" ? (g(), b(le, { key: 0 }, [
                w("span", {
                  class: h(E.$style.ghostLabel)
                }, x(((O = i.value.get(c.value.payload.nodeId)) == null ? void 0 : O.label) ?? "Node"), 3),
                w("span", {
                  class: h(E.$style.ghostType)
                }, x(((y = i.value.get(c.value.payload.nodeId)) == null ? void 0 : y.nodeType) ?? ""), 3)
              ], 64)) : c.value.payload.kind === "palette" ? (g(), b(le, { key: 1 }, [
                w("span", {
                  class: h(E.$style.ghostLabel)
                }, x(c.value.payload.item.label), 3),
                w("span", {
                  class: h(E.$style.ghostType)
                }, x(c.value.payload.item.nodeType), 3)
              ], 64)) : L("", !0)
            ], 2)
          ], 6)) : L("", !0)
        ]))
      ], 2);
    };
  }
}), og = "_scroll_1luup_2", ig = "_inner_1luup_21", lg = "_slot_1luup_33", cg = "_phantom_1luup_47", dg = "_phantomCard_1luup_52", ug = "_phantomLabel_1luup_65", hg = "_phantomType_1luup_70", pg = "_ghost_1luup_79", fg = "_ghostInner_1luup_90", gg = "_ghostLabel_1luup_101", yg = "_ghostType_1luup_107", bg = {
  scroll: og,
  inner: ig,
  slot: lg,
  phantom: cg,
  phantomCard: dg,
  phantomLabel: ug,
  phantomType: hg,
  ghost: pg,
  ghostInner: fg,
  ghostLabel: gg,
  ghostType: yg
}, mg = {
  $style: bg
}, _m = /* @__PURE__ */ D(ag, [["__cssModules", mg]]), kg = ["onPointerdown"], _g = /* @__PURE__ */ z({
  __name: "NodePalette",
  props: {
    open: { type: Boolean },
    items: {},
    title: { default: "Add node" },
    width: { default: 280 }
  },
  emits: ["close", "start-drag"],
  setup(e, { emit: t }) {
    const n = t;
    let s = null;
    function r(a, u) {
      s = { item: u, originX: a.clientX, originY: a.clientY }, window.addEventListener("pointermove", o), window.addEventListener("pointerup", l), window.addEventListener("pointercancel", l);
    }
    function o(a) {
      s && Es(a.clientX, a.clientY, s.originX, s.originY) && (n("start-drag", s.item, a), i());
    }
    function l() {
      i();
    }
    function i() {
      s = null, window.removeEventListener("pointermove", o), window.removeEventListener("pointerup", l), window.removeEventListener("pointercancel", l);
    }
    return (a, u) => (g(), ce(rr, {
      name: a.$style.slide
    }, {
      default: be(() => [
        e.open ? (g(), b("aside", {
          key: 0,
          class: h(a.$style.panel),
          style: Se({ width: e.width + "px" }),
          role: "dialog",
          "aria-label": "Add node"
        }, [
          w("header", {
            class: h(a.$style.head)
          }, [
            w("span", {
              class: h(a.$style.title)
            }, x(e.title), 3),
            w("button", {
              type: "button",
              class: h(a.$style.closeBtn),
              "aria-label": "Close palette",
              onClick: u[0] || (u[0] = (c) => n("close"))
            }, "×", 2)
          ], 2),
          w("div", {
            class: h(a.$style.body)
          }, [
            w("p", {
              class: h(a.$style.hint)
            }, [
              w("span", {
                class: h(a.$style.comment)
              }, "// drag onto the canvas", 2)
            ], 2),
            w("ul", {
              class: h(a.$style.list)
            }, [
              (g(!0), b(le, null, ye(e.items, (c) => (g(), b("li", {
                key: c.id,
                class: h(a.$style.item),
                onPointerdown: (p) => r(p, c)
              }, [
                w("div", {
                  class: h(a.$style.itemHead)
                }, [
                  w("span", {
                    class: h(a.$style.itemLabel)
                  }, x(c.label), 3),
                  w("span", {
                    class: h(a.$style.itemType)
                  }, x(c.nodeType), 3)
                ], 2),
                c.description ? (g(), b("p", {
                  key: 0,
                  class: h(a.$style.itemDesc)
                }, x(c.description), 3)) : L("", !0)
              ], 42, kg))), 128))
            ], 2)
          ], 2)
        ], 6)) : L("", !0)
      ]),
      _: 1
    }, 8, ["name"]));
  }
}), wg = "_panel_djxcr_2", $g = "_head_djxcr_16", vg = "_title_djxcr_25", Eg = "_closeBtn_djxcr_27", Sg = "_body_djxcr_39", Tg = "_hint_djxcr_48", xg = "_comment_djxcr_49", Mg = "_list_djxcr_55", Ag = "_item_djxcr_64", Cg = "_itemHead_djxcr_84", Ig = "_itemLabel_djxcr_91", Og = "_itemType_djxcr_93", Ng = "_itemDesc_djxcr_101", Rg = {
  panel: wg,
  head: $g,
  title: vg,
  closeBtn: Eg,
  body: Sg,
  hint: Tg,
  comment: xg,
  list: Mg,
  item: Ag,
  itemHead: Cg,
  itemLabel: Ig,
  itemType: Og,
  itemDesc: Ng,
  "slide-enter-from": "_slide-enter-from_djxcr_108",
  "slide-leave-to": "_slide-leave-to_djxcr_108",
  "slide-enter-active": "_slide-enter-active_djxcr_109",
  "slide-leave-active": "_slide-leave-active_djxcr_109",
  "slide-enter-to": "_slide-enter-to_djxcr_110",
  "slide-leave-from": "_slide-leave-from_djxcr_110"
}, Lg = {
  $style: Rg
}, wm = /* @__PURE__ */ D(_g, [["__cssModules", Lg]]);
async function X(e) {
  const { data: t, error: n, response: s } = await e;
  if (n !== void 0 || !(s != null && s.ok)) {
    const r = (n == null ? void 0 : n.message) ?? (s ? `${s.status} ${s.statusText}` : "Network error");
    throw new Error(r);
  }
  return t;
}
function Wt(e, t) {
  const n = {};
  for (const [s, r] of Object.entries(e))
    n[s] = typeof r == "function" ? (o) => r({ ...o, client: t }) : r;
  return n;
}
const Bg = {
  bodySerializer: (e) => JSON.stringify(e, (t, n) => typeof n == "bigint" ? n.toString() : n)
};
function zg({
  onRequest: e,
  onSseError: t,
  onSseEvent: n,
  responseTransformer: s,
  responseValidator: r,
  sseDefaultRetryDelay: o,
  sseMaxRetryAttempts: l,
  sseMaxRetryDelay: i,
  sseSleepFn: a,
  url: u,
  ...c
}) {
  let p;
  const d = a ?? ((k) => new Promise(($) => setTimeout($, k)));
  return { stream: async function* () {
    let k = o ?? 3e3, $ = 0;
    const I = c.signal ?? new AbortController().signal;
    for (; !I.aborted; ) {
      $++;
      const P = c.headers instanceof Headers ? c.headers : new Headers(c.headers);
      p !== void 0 && P.set("Last-Event-ID", p);
      try {
        const R = {
          redirect: "follow",
          ...c,
          body: c.serializedBody,
          headers: P,
          signal: I
        };
        let B = new Request(u, R);
        e && (B = await e(u, R));
        const M = await (c.fetch ?? globalThis.fetch)(B);
        if (!M.ok) throw new Error(`SSE failed: ${M.status} ${M.statusText}`);
        if (!M.body) throw new Error("No body in SSE response");
        const q = M.body.pipeThrough(new TextDecoderStream()).getReader();
        let T = "";
        const te = () => {
          try {
            q.cancel();
          } catch {
          }
        };
        I.addEventListener("abort", te);
        try {
          for (; ; ) {
            const { done: re, value: de } = await q.read();
            if (re) break;
            T += de, T = T.replace(/\r\n?/g, `
`);
            const se = T.split(`

`);
            T = se.pop() ?? "";
            for (const E of se) {
              const C = E.split(`
`), O = [];
              let y;
              for (const v of C)
                if (v.startsWith("data:"))
                  O.push(v.replace(/^data:\s*/, ""));
                else if (v.startsWith("event:"))
                  y = v.replace(/^event:\s*/, "");
                else if (v.startsWith("id:"))
                  p = v.replace(/^id:\s*/, "");
                else if (v.startsWith("retry:")) {
                  const U = Number.parseInt(v.replace(/^retry:\s*/, ""), 10);
                  Number.isNaN(U) || (k = U);
                }
              let m, S = !1;
              if (O.length) {
                const v = O.join(`
`);
                try {
                  m = JSON.parse(v), S = !0;
                } catch {
                  m = v;
                }
              }
              S && (r && await r(m), s && (m = await s(m))), n == null || n({
                data: m,
                event: y,
                id: p,
                retry: k
              }), O.length && (yield m);
            }
          }
        } finally {
          I.removeEventListener("abort", te), q.releaseLock();
        }
        break;
      } catch (R) {
        if (t == null || t(R), l !== void 0 && $ >= l)
          break;
        const B = Math.min(k * 2 ** ($ - 1), i ?? 3e4);
        await d(B);
      }
    }
  }() };
}
const Dg = (e) => {
  switch (e) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, Pg = (e) => {
  switch (e) {
    case "form":
      return ",";
    case "pipeDelimited":
      return "|";
    case "spaceDelimited":
      return "%20";
    default:
      return ",";
  }
}, Ug = (e) => {
  switch (e) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, Ss = ({
  allowReserved: e,
  explode: t,
  name: n,
  style: s,
  value: r
}) => {
  if (!t) {
    const i = (e ? r : r.map((a) => encodeURIComponent(a))).join(Pg(s));
    switch (s) {
      case "label":
        return `.${i}`;
      case "matrix":
        return `;${n}=${i}`;
      case "simple":
        return i;
      default:
        return `${n}=${i}`;
    }
  }
  const o = Dg(s), l = r.map((i) => s === "label" || s === "simple" ? e ? i : encodeURIComponent(i) : pt({
    allowReserved: e,
    name: n,
    value: i
  })).join(o);
  return s === "label" || s === "matrix" ? o + l : l;
}, pt = ({
  allowReserved: e,
  name: t,
  value: n
}) => {
  if (n == null)
    return "";
  if (typeof n == "object")
    throw new Error(
      "Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these."
    );
  return `${t}=${e ? n : encodeURIComponent(n)}`;
}, Ts = ({
  allowReserved: e,
  explode: t,
  name: n,
  style: s,
  value: r,
  valueOnly: o
}) => {
  if (r instanceof Date)
    return o ? r.toISOString() : `${n}=${r.toISOString()}`;
  if (s !== "deepObject" && !t) {
    let a = [];
    Object.entries(r).forEach(([c, p]) => {
      a = [...a, c, e ? p : encodeURIComponent(p)];
    });
    const u = a.join(",");
    switch (s) {
      case "form":
        return `${n}=${u}`;
      case "label":
        return `.${u}`;
      case "matrix":
        return `;${n}=${u}`;
      default:
        return u;
    }
  }
  const l = Ug(s), i = Object.entries(r).map(
    ([a, u]) => pt({
      allowReserved: e,
      name: s === "deepObject" ? `${n}[${a}]` : a,
      value: u
    })
  ).join(l);
  return s === "label" || s === "matrix" ? l + i : i;
}, qg = /\{[^{}]+\}/g, jg = ({ path: e, url: t }) => {
  let n = t;
  const s = t.match(qg);
  if (s)
    for (const r of s) {
      let o = !1, l = r.substring(1, r.length - 1), i = "simple";
      l.endsWith("*") && (o = !0, l = l.substring(0, l.length - 1)), l.startsWith(".") ? (l = l.substring(1), i = "label") : l.startsWith(";") && (l = l.substring(1), i = "matrix");
      const a = e[l];
      if (a == null)
        continue;
      if (Array.isArray(a)) {
        n = n.replace(r, Ss({ explode: o, name: l, style: i, value: a }));
        continue;
      }
      if (typeof a == "object") {
        n = n.replace(
          r,
          Ts({
            explode: o,
            name: l,
            style: i,
            value: a,
            valueOnly: !0
          })
        );
        continue;
      }
      if (i === "matrix") {
        n = n.replace(
          r,
          `;${pt({
            name: l,
            value: a
          })}`
        );
        continue;
      }
      const u = encodeURIComponent(
        i === "label" ? `.${a}` : a
      );
      n = n.replace(r, u);
    }
  return n;
}, Hg = ({
  baseUrl: e,
  path: t,
  query: n,
  querySerializer: s,
  url: r
}) => {
  const o = r.startsWith("/") ? r : `/${r}`;
  let l = (e ?? "") + o;
  t && (l = jg({ path: t, url: l }));
  let i = n ? s(n) : "";
  return i.startsWith("?") && (i = i.substring(1)), i && (l += `?${i}`), l;
};
function $n(e) {
  const t = e.body !== void 0;
  if (t && e.bodySerializer)
    return "serializedBody" in e ? e.serializedBody !== void 0 && e.serializedBody !== "" ? e.serializedBody : null : e.body !== "" ? e.body : null;
  if (t)
    return e.body;
}
const Vg = async (e, t) => {
  const n = typeof t == "function" ? await t(e) : t;
  if (n)
    return e.scheme === "bearer" ? `Bearer ${n}` : e.scheme === "basic" ? `Basic ${btoa(n)}` : n;
}, xs = ({
  parameters: e = {},
  ...t
} = {}) => (s) => {
  const r = [];
  if (s && typeof s == "object")
    for (const o in s) {
      const l = s[o];
      if (l == null)
        continue;
      const i = e[o] || t;
      if (Array.isArray(l)) {
        const a = Ss({
          allowReserved: i.allowReserved,
          explode: !0,
          name: o,
          style: "form",
          value: l,
          ...i.array
        });
        a && r.push(a);
      } else if (typeof l == "object") {
        const a = Ts({
          allowReserved: i.allowReserved,
          explode: !0,
          name: o,
          style: "deepObject",
          value: l,
          ...i.object
        });
        a && r.push(a);
      } else {
        const a = pt({
          allowReserved: i.allowReserved,
          name: o,
          value: l
        });
        a && r.push(a);
      }
    }
  return r.join("&");
}, Fg = (e) => {
  var n;
  if (!e)
    return "stream";
  const t = (n = e.split(";")[0]) == null ? void 0 : n.trim();
  if (t) {
    if (t.startsWith("application/json") || t.endsWith("+json"))
      return "json";
    if (t === "multipart/form-data")
      return "formData";
    if (["application/", "audio/", "image/", "video/"].some((s) => t.startsWith(s)))
      return "blob";
    if (t.startsWith("text/"))
      return "text";
  }
}, Gg = (e, t) => {
  var n, s;
  return t ? !!(e.headers.has(t) || (n = e.query) != null && n[t] || (s = e.headers.get("Cookie")) != null && s.includes(`${t}=`)) : !1;
};
async function Wg(e) {
  for (const t of e.security ?? []) {
    if (Gg(e, t.name))
      continue;
    const n = await Vg(t, e.auth);
    if (!n)
      continue;
    const s = t.name ?? "Authorization";
    switch (t.in) {
      case "query":
        e.query || (e.query = {}), e.query[s] = n;
        break;
      case "cookie":
        e.headers.append("Cookie", `${s}=${n}`);
        break;
      case "header":
      default:
        e.headers.set(s, n);
        break;
    }
  }
}
const vn = (e) => Hg({
  baseUrl: e.baseUrl,
  path: e.path,
  query: e.query,
  querySerializer: typeof e.querySerializer == "function" ? e.querySerializer : xs(e.querySerializer),
  url: e.url
}), En = (e, t) => {
  var s;
  const n = { ...e, ...t };
  return (s = n.baseUrl) != null && s.endsWith("/") && (n.baseUrl = n.baseUrl.substring(0, n.baseUrl.length - 1)), n.headers = Ms(e.headers, t.headers), n;
}, Kg = (e) => {
  const t = [];
  return e.forEach((n, s) => {
    t.push([s, n]);
  }), t;
}, Ms = (...e) => {
  const t = new Headers();
  for (const n of e) {
    if (!n)
      continue;
    const s = n instanceof Headers ? Kg(n) : Object.entries(n);
    for (const [r, o] of s)
      if (o === null)
        t.delete(r);
      else if (Array.isArray(o))
        for (const l of o)
          t.append(r, l);
      else o !== void 0 && t.set(
        r,
        typeof o == "object" ? JSON.stringify(o) : o
      );
  }
  return t;
};
let wt = class {
  constructor() {
    G(this, "fns", []);
  }
  clear() {
    this.fns = [];
  }
  eject(t) {
    const n = this.getInterceptorIndex(t);
    this.fns[n] && (this.fns[n] = null);
  }
  exists(t) {
    const n = this.getInterceptorIndex(t);
    return !!this.fns[n];
  }
  getInterceptorIndex(t) {
    return typeof t == "number" ? this.fns[t] ? t : -1 : this.fns.indexOf(t);
  }
  update(t, n) {
    const s = this.getInterceptorIndex(t);
    return this.fns[s] ? (this.fns[s] = n, t) : !1;
  }
  use(t) {
    return this.fns.push(t), this.fns.length - 1;
  }
};
const Zg = () => ({
  error: new wt(),
  request: new wt(),
  response: new wt()
}), Xg = xs({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), Yg = {
  "Content-Type": "application/json"
}, Kt = (e = {}) => ({
  ...Bg,
  headers: Yg,
  parseAs: "auto",
  querySerializer: Xg,
  ...e
}), As = (e = {}) => {
  let t = En(Kt(), e);
  const n = () => ({ ...t }), s = (c) => (t = En(t, c), n()), r = Zg(), o = async (c) => {
    const p = {
      ...t,
      ...c,
      fetch: c.fetch ?? t.fetch ?? globalThis.fetch,
      headers: Ms(t.headers, c.headers),
      serializedBody: void 0
    };
    p.security && await Wg(p), p.requestValidator && await p.requestValidator(p), p.body !== void 0 && p.bodySerializer && (p.serializedBody = p.bodySerializer(p.body)), (p.body === void 0 || p.serializedBody === "") && p.headers.delete("Content-Type");
    const d = p, _ = vn(d);
    return { opts: d, url: _ };
  }, l = async (c) => {
    const p = c.throwOnError ?? t.throwOnError, d = c.responseStyle ?? t.responseStyle;
    let _, f;
    try {
      const { opts: k, url: $ } = await o(c), I = {
        redirect: "follow",
        ...k,
        body: $n(k)
      };
      _ = new Request($, I);
      for (const M of r.request.fns)
        M && (_ = await M(_, k));
      const P = k.fetch;
      f = await P(_);
      for (const M of r.response.fns)
        M && (f = await M(f, _, k));
      const R = {
        request: _,
        response: f
      };
      if (f.ok) {
        const M = (k.parseAs === "auto" ? Fg(f.headers.get("Content-Type")) : k.parseAs) ?? "json";
        if (f.status === 204 || f.headers.get("Content-Length") === "0") {
          let T;
          switch (M) {
            case "arrayBuffer":
            case "blob":
            case "text":
              T = await f[M]();
              break;
            case "formData":
              T = new FormData();
              break;
            case "stream":
              T = f.body;
              break;
            case "json":
            default:
              T = {};
              break;
          }
          return k.responseStyle === "data" ? T : {
            data: T,
            ...R
          };
        }
        let q;
        switch (M) {
          case "arrayBuffer":
          case "blob":
          case "formData":
          case "text":
            q = await f[M]();
            break;
          case "json": {
            const T = await f.text();
            q = T ? JSON.parse(T) : {};
            break;
          }
          case "stream":
            return k.responseStyle === "data" ? f.body : {
              data: f.body,
              ...R
            };
        }
        return M === "json" && (k.responseValidator && await k.responseValidator(q), k.responseTransformer && (q = await k.responseTransformer(q))), k.responseStyle === "data" ? q : {
          data: q,
          ...R
        };
      }
      const B = await f.text();
      let F;
      try {
        F = JSON.parse(B);
      } catch {
      }
      throw F ?? B;
    } catch (k) {
      let $ = k;
      for (const I of r.error.fns)
        I && ($ = await I($, f, _, c));
      if ($ = $ || {}, p)
        throw $;
      return d === "data" ? void 0 : {
        error: $,
        request: _,
        response: f
      };
    }
  }, i = (c) => (p) => l({ ...p, method: c }), a = (c) => async (p) => {
    const { opts: d, url: _ } = await o(p);
    return zg({
      ...d,
      body: d.body,
      method: c,
      onRequest: async (f, k) => {
        let $ = new Request(f, k);
        for (const I of r.request.fns)
          I && ($ = await I($, d));
        return $;
      },
      serializedBody: $n(d),
      url: _
    });
  };
  return {
    buildUrl: (c) => vn({ ...t, ...c }),
    connect: i("CONNECT"),
    delete: i("DELETE"),
    get: i("GET"),
    getConfig: n,
    head: i("HEAD"),
    interceptors: r,
    options: i("OPTIONS"),
    patch: i("PATCH"),
    post: i("POST"),
    put: i("PUT"),
    request: l,
    setConfig: s,
    sse: {
      connect: a("CONNECT"),
      delete: a("DELETE"),
      get: a("GET"),
      head: a("HEAD"),
      options: a("OPTIONS"),
      patch: a("PATCH"),
      post: a("POST"),
      put: a("PUT"),
      trace: a("TRACE")
    },
    trace: i("TRACE")
  };
}, Te = As(Kt()), Jg = (e) => ((e == null ? void 0 : e.client) ?? Te).get({ url: "/ping", ...e }), Qg = (e) => ((e == null ? void 0 : e.client) ?? Te).get({ url: "/tools", ...e }), ey = (e) => ((e == null ? void 0 : e.client) ?? Te).get({ url: "/tools/grouped", ...e }), ty = (e) => ((e == null ? void 0 : e.client) ?? Te).get({ url: "/tools/all", ...e }), ny = (e) => ((e == null ? void 0 : e.client) ?? Te).post({ url: "/tools/rebuild", ...e }), sy = (e) => (e.client ?? Te).post({ url: "/tools/{name}/enable", ...e }), ry = (e) => (e.client ?? Te).post({ url: "/tools/{name}/disable", ...e }), ay = (e) => ((e == null ? void 0 : e.client) ?? Te).get({ url: "/toolboxes/", ...e }), oy = (e) => (e.client ?? Te).post({
  url: "/toolboxes/mcp",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), iy = (e) => (e.client ?? Te).delete({ url: "/toolboxes/{id}", ...e }), ly = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  deleteToolboxesById: iy,
  getPing: Jg,
  getToolboxes: ay,
  getTools: Qg,
  getToolsAll: ty,
  getToolsGrouped: ey,
  postToolboxesMcp: oy,
  postToolsByNameDisable: ry,
  postToolsByNameEnable: sy,
  postToolsRebuild: ny
}, Symbol.toStringTag, { value: "Module" })), cy = {
  bodySerializer: (e) => JSON.stringify(e, (t, n) => typeof n == "bigint" ? n.toString() : n)
};
function dy({
  onRequest: e,
  onSseError: t,
  onSseEvent: n,
  responseTransformer: s,
  responseValidator: r,
  sseDefaultRetryDelay: o,
  sseMaxRetryAttempts: l,
  sseMaxRetryDelay: i,
  sseSleepFn: a,
  url: u,
  ...c
}) {
  let p;
  const d = a ?? ((k) => new Promise(($) => setTimeout($, k)));
  return { stream: async function* () {
    let k = o ?? 3e3, $ = 0;
    const I = c.signal ?? new AbortController().signal;
    for (; !I.aborted; ) {
      $++;
      const P = c.headers instanceof Headers ? c.headers : new Headers(c.headers);
      p !== void 0 && P.set("Last-Event-ID", p);
      try {
        const R = {
          redirect: "follow",
          ...c,
          body: c.serializedBody,
          headers: P,
          signal: I
        };
        let B = new Request(u, R);
        e && (B = await e(u, R));
        const M = await (c.fetch ?? globalThis.fetch)(B);
        if (!M.ok) throw new Error(`SSE failed: ${M.status} ${M.statusText}`);
        if (!M.body) throw new Error("No body in SSE response");
        const q = M.body.pipeThrough(new TextDecoderStream()).getReader();
        let T = "";
        const te = () => {
          try {
            q.cancel();
          } catch {
          }
        };
        I.addEventListener("abort", te);
        try {
          for (; ; ) {
            const { done: re, value: de } = await q.read();
            if (re) break;
            T += de, T = T.replace(/\r\n?/g, `
`);
            const se = T.split(`

`);
            T = se.pop() ?? "";
            for (const E of se) {
              const C = E.split(`
`), O = [];
              let y;
              for (const v of C)
                if (v.startsWith("data:"))
                  O.push(v.replace(/^data:\s*/, ""));
                else if (v.startsWith("event:"))
                  y = v.replace(/^event:\s*/, "");
                else if (v.startsWith("id:"))
                  p = v.replace(/^id:\s*/, "");
                else if (v.startsWith("retry:")) {
                  const U = Number.parseInt(v.replace(/^retry:\s*/, ""), 10);
                  Number.isNaN(U) || (k = U);
                }
              let m, S = !1;
              if (O.length) {
                const v = O.join(`
`);
                try {
                  m = JSON.parse(v), S = !0;
                } catch {
                  m = v;
                }
              }
              S && (r && await r(m), s && (m = await s(m))), n == null || n({
                data: m,
                event: y,
                id: p,
                retry: k
              }), O.length && (yield m);
            }
          }
        } finally {
          I.removeEventListener("abort", te), q.releaseLock();
        }
        break;
      } catch (R) {
        if (t == null || t(R), l !== void 0 && $ >= l)
          break;
        const B = Math.min(k * 2 ** ($ - 1), i ?? 3e4);
        await d(B);
      }
    }
  }() };
}
const uy = (e) => {
  switch (e) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, hy = (e) => {
  switch (e) {
    case "form":
      return ",";
    case "pipeDelimited":
      return "|";
    case "spaceDelimited":
      return "%20";
    default:
      return ",";
  }
}, py = (e) => {
  switch (e) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, Cs = ({
  allowReserved: e,
  explode: t,
  name: n,
  style: s,
  value: r
}) => {
  if (!t) {
    const i = (e ? r : r.map((a) => encodeURIComponent(a))).join(hy(s));
    switch (s) {
      case "label":
        return `.${i}`;
      case "matrix":
        return `;${n}=${i}`;
      case "simple":
        return i;
      default:
        return `${n}=${i}`;
    }
  }
  const o = uy(s), l = r.map((i) => s === "label" || s === "simple" ? e ? i : encodeURIComponent(i) : ft({
    allowReserved: e,
    name: n,
    value: i
  })).join(o);
  return s === "label" || s === "matrix" ? o + l : l;
}, ft = ({
  allowReserved: e,
  name: t,
  value: n
}) => {
  if (n == null)
    return "";
  if (typeof n == "object")
    throw new Error(
      "Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these."
    );
  return `${t}=${e ? n : encodeURIComponent(n)}`;
}, Is = ({
  allowReserved: e,
  explode: t,
  name: n,
  style: s,
  value: r,
  valueOnly: o
}) => {
  if (r instanceof Date)
    return o ? r.toISOString() : `${n}=${r.toISOString()}`;
  if (s !== "deepObject" && !t) {
    let a = [];
    Object.entries(r).forEach(([c, p]) => {
      a = [...a, c, e ? p : encodeURIComponent(p)];
    });
    const u = a.join(",");
    switch (s) {
      case "form":
        return `${n}=${u}`;
      case "label":
        return `.${u}`;
      case "matrix":
        return `;${n}=${u}`;
      default:
        return u;
    }
  }
  const l = py(s), i = Object.entries(r).map(
    ([a, u]) => ft({
      allowReserved: e,
      name: s === "deepObject" ? `${n}[${a}]` : a,
      value: u
    })
  ).join(l);
  return s === "label" || s === "matrix" ? l + i : i;
}, fy = /\{[^{}]+\}/g, gy = ({ path: e, url: t }) => {
  let n = t;
  const s = t.match(fy);
  if (s)
    for (const r of s) {
      let o = !1, l = r.substring(1, r.length - 1), i = "simple";
      l.endsWith("*") && (o = !0, l = l.substring(0, l.length - 1)), l.startsWith(".") ? (l = l.substring(1), i = "label") : l.startsWith(";") && (l = l.substring(1), i = "matrix");
      const a = e[l];
      if (a == null)
        continue;
      if (Array.isArray(a)) {
        n = n.replace(r, Cs({ explode: o, name: l, style: i, value: a }));
        continue;
      }
      if (typeof a == "object") {
        n = n.replace(
          r,
          Is({
            explode: o,
            name: l,
            style: i,
            value: a,
            valueOnly: !0
          })
        );
        continue;
      }
      if (i === "matrix") {
        n = n.replace(
          r,
          `;${ft({
            name: l,
            value: a
          })}`
        );
        continue;
      }
      const u = encodeURIComponent(
        i === "label" ? `.${a}` : a
      );
      n = n.replace(r, u);
    }
  return n;
}, yy = ({
  baseUrl: e,
  path: t,
  query: n,
  querySerializer: s,
  url: r
}) => {
  const o = r.startsWith("/") ? r : `/${r}`;
  let l = (e ?? "") + o;
  t && (l = gy({ path: t, url: l }));
  let i = n ? s(n) : "";
  return i.startsWith("?") && (i = i.substring(1)), i && (l += `?${i}`), l;
};
function Sn(e) {
  const t = e.body !== void 0;
  if (t && e.bodySerializer)
    return "serializedBody" in e ? e.serializedBody !== void 0 && e.serializedBody !== "" ? e.serializedBody : null : e.body !== "" ? e.body : null;
  if (t)
    return e.body;
}
const by = async (e, t) => {
  const n = typeof t == "function" ? await t(e) : t;
  if (n)
    return e.scheme === "bearer" ? `Bearer ${n}` : e.scheme === "basic" ? `Basic ${btoa(n)}` : n;
}, Os = ({
  parameters: e = {},
  ...t
} = {}) => (s) => {
  const r = [];
  if (s && typeof s == "object")
    for (const o in s) {
      const l = s[o];
      if (l == null)
        continue;
      const i = e[o] || t;
      if (Array.isArray(l)) {
        const a = Cs({
          allowReserved: i.allowReserved,
          explode: !0,
          name: o,
          style: "form",
          value: l,
          ...i.array
        });
        a && r.push(a);
      } else if (typeof l == "object") {
        const a = Is({
          allowReserved: i.allowReserved,
          explode: !0,
          name: o,
          style: "deepObject",
          value: l,
          ...i.object
        });
        a && r.push(a);
      } else {
        const a = ft({
          allowReserved: i.allowReserved,
          name: o,
          value: l
        });
        a && r.push(a);
      }
    }
  return r.join("&");
}, my = (e) => {
  var n;
  if (!e)
    return "stream";
  const t = (n = e.split(";")[0]) == null ? void 0 : n.trim();
  if (t) {
    if (t.startsWith("application/json") || t.endsWith("+json"))
      return "json";
    if (t === "multipart/form-data")
      return "formData";
    if (["application/", "audio/", "image/", "video/"].some((s) => t.startsWith(s)))
      return "blob";
    if (t.startsWith("text/"))
      return "text";
  }
}, ky = (e, t) => {
  var n, s;
  return t ? !!(e.headers.has(t) || (n = e.query) != null && n[t] || (s = e.headers.get("Cookie")) != null && s.includes(`${t}=`)) : !1;
};
async function _y(e) {
  for (const t of e.security ?? []) {
    if (ky(e, t.name))
      continue;
    const n = await by(t, e.auth);
    if (!n)
      continue;
    const s = t.name ?? "Authorization";
    switch (t.in) {
      case "query":
        e.query || (e.query = {}), e.query[s] = n;
        break;
      case "cookie":
        e.headers.append("Cookie", `${s}=${n}`);
        break;
      case "header":
      default:
        e.headers.set(s, n);
        break;
    }
  }
}
const Tn = (e) => yy({
  baseUrl: e.baseUrl,
  path: e.path,
  query: e.query,
  querySerializer: typeof e.querySerializer == "function" ? e.querySerializer : Os(e.querySerializer),
  url: e.url
}), xn = (e, t) => {
  var s;
  const n = { ...e, ...t };
  return (s = n.baseUrl) != null && s.endsWith("/") && (n.baseUrl = n.baseUrl.substring(0, n.baseUrl.length - 1)), n.headers = Ns(e.headers, t.headers), n;
}, wy = (e) => {
  const t = [];
  return e.forEach((n, s) => {
    t.push([s, n]);
  }), t;
}, Ns = (...e) => {
  const t = new Headers();
  for (const n of e) {
    if (!n)
      continue;
    const s = n instanceof Headers ? wy(n) : Object.entries(n);
    for (const [r, o] of s)
      if (o === null)
        t.delete(r);
      else if (Array.isArray(o))
        for (const l of o)
          t.append(r, l);
      else o !== void 0 && t.set(
        r,
        typeof o == "object" ? JSON.stringify(o) : o
      );
  }
  return t;
};
let $t = class {
  constructor() {
    G(this, "fns", []);
  }
  clear() {
    this.fns = [];
  }
  eject(t) {
    const n = this.getInterceptorIndex(t);
    this.fns[n] && (this.fns[n] = null);
  }
  exists(t) {
    const n = this.getInterceptorIndex(t);
    return !!this.fns[n];
  }
  getInterceptorIndex(t) {
    return typeof t == "number" ? this.fns[t] ? t : -1 : this.fns.indexOf(t);
  }
  update(t, n) {
    const s = this.getInterceptorIndex(t);
    return this.fns[s] ? (this.fns[s] = n, t) : !1;
  }
  use(t) {
    return this.fns.push(t), this.fns.length - 1;
  }
};
const $y = () => ({
  error: new $t(),
  request: new $t(),
  response: new $t()
}), vy = Os({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), Ey = {
  "Content-Type": "application/json"
}, Zt = (e = {}) => ({
  ...cy,
  headers: Ey,
  parseAs: "auto",
  querySerializer: vy,
  ...e
}), Rs = (e = {}) => {
  let t = xn(Zt(), e);
  const n = () => ({ ...t }), s = (c) => (t = xn(t, c), n()), r = $y(), o = async (c) => {
    const p = {
      ...t,
      ...c,
      fetch: c.fetch ?? t.fetch ?? globalThis.fetch,
      headers: Ns(t.headers, c.headers),
      serializedBody: void 0
    };
    p.security && await _y(p), p.requestValidator && await p.requestValidator(p), p.body !== void 0 && p.bodySerializer && (p.serializedBody = p.bodySerializer(p.body)), (p.body === void 0 || p.serializedBody === "") && p.headers.delete("Content-Type");
    const d = p, _ = Tn(d);
    return { opts: d, url: _ };
  }, l = async (c) => {
    const p = c.throwOnError ?? t.throwOnError, d = c.responseStyle ?? t.responseStyle;
    let _, f;
    try {
      const { opts: k, url: $ } = await o(c), I = {
        redirect: "follow",
        ...k,
        body: Sn(k)
      };
      _ = new Request($, I);
      for (const M of r.request.fns)
        M && (_ = await M(_, k));
      const P = k.fetch;
      f = await P(_);
      for (const M of r.response.fns)
        M && (f = await M(f, _, k));
      const R = {
        request: _,
        response: f
      };
      if (f.ok) {
        const M = (k.parseAs === "auto" ? my(f.headers.get("Content-Type")) : k.parseAs) ?? "json";
        if (f.status === 204 || f.headers.get("Content-Length") === "0") {
          let T;
          switch (M) {
            case "arrayBuffer":
            case "blob":
            case "text":
              T = await f[M]();
              break;
            case "formData":
              T = new FormData();
              break;
            case "stream":
              T = f.body;
              break;
            case "json":
            default:
              T = {};
              break;
          }
          return k.responseStyle === "data" ? T : {
            data: T,
            ...R
          };
        }
        let q;
        switch (M) {
          case "arrayBuffer":
          case "blob":
          case "formData":
          case "text":
            q = await f[M]();
            break;
          case "json": {
            const T = await f.text();
            q = T ? JSON.parse(T) : {};
            break;
          }
          case "stream":
            return k.responseStyle === "data" ? f.body : {
              data: f.body,
              ...R
            };
        }
        return M === "json" && (k.responseValidator && await k.responseValidator(q), k.responseTransformer && (q = await k.responseTransformer(q))), k.responseStyle === "data" ? q : {
          data: q,
          ...R
        };
      }
      const B = await f.text();
      let F;
      try {
        F = JSON.parse(B);
      } catch {
      }
      throw F ?? B;
    } catch (k) {
      let $ = k;
      for (const I of r.error.fns)
        I && ($ = await I($, f, _, c));
      if ($ = $ || {}, p)
        throw $;
      return d === "data" ? void 0 : {
        error: $,
        request: _,
        response: f
      };
    }
  }, i = (c) => (p) => l({ ...p, method: c }), a = (c) => async (p) => {
    const { opts: d, url: _ } = await o(p);
    return dy({
      ...d,
      body: d.body,
      method: c,
      onRequest: async (f, k) => {
        let $ = new Request(f, k);
        for (const I of r.request.fns)
          I && ($ = await I($, d));
        return $;
      },
      serializedBody: Sn(d),
      url: _
    });
  };
  return {
    buildUrl: (c) => Tn({ ...t, ...c }),
    connect: i("CONNECT"),
    delete: i("DELETE"),
    get: i("GET"),
    getConfig: n,
    head: i("HEAD"),
    interceptors: r,
    options: i("OPTIONS"),
    patch: i("PATCH"),
    post: i("POST"),
    put: i("PUT"),
    request: l,
    setConfig: s,
    sse: {
      connect: a("CONNECT"),
      delete: a("DELETE"),
      get: a("GET"),
      head: a("HEAD"),
      options: a("OPTIONS"),
      patch: a("PATCH"),
      post: a("POST"),
      put: a("PUT"),
      trace: a("TRACE")
    },
    trace: i("TRACE")
  };
}, xe = Rs(Zt()), Sy = (e) => ((e == null ? void 0 : e.client) ?? xe).get({ url: "/ping", ...e }), Ty = (e) => ((e == null ? void 0 : e.client) ?? xe).get({ url: "/models", ...e }), xy = (e) => ((e == null ? void 0 : e.client) ?? xe).get({ url: "/hooks/", ...e }), My = (e) => (e.client ?? xe).post({
  url: "/hooks/",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), Ay = (e) => (e.client ?? xe).delete({ url: "/hooks/{id}", ...e }), Cy = (e) => ((e == null ? void 0 : e.client) ?? xe).delete({ url: "/config", ...e }), Iy = (e) => ((e == null ? void 0 : e.client) ?? xe).get({ url: "/config", ...e }), Oy = (e) => (e.client ?? xe).post({
  url: "/config",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), Ny = (e) => (e.client ?? xe).post({
  url: "/chat",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), Ry = (e) => (e.client ?? xe).post({
  url: "/agent",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), Ly = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  deleteConfig: Cy,
  deleteHooksById: Ay,
  getConfig: Iy,
  getHooks: xy,
  getModels: Ty,
  getPing: Sy,
  postAgent: Ry,
  postChat: Ny,
  postConfig: Oy,
  postHooks: My
}, Symbol.toStringTag, { value: "Module" })), By = {
  bodySerializer: (e) => JSON.stringify(e, (t, n) => typeof n == "bigint" ? n.toString() : n)
};
function zy({
  onRequest: e,
  onSseError: t,
  onSseEvent: n,
  responseTransformer: s,
  responseValidator: r,
  sseDefaultRetryDelay: o,
  sseMaxRetryAttempts: l,
  sseMaxRetryDelay: i,
  sseSleepFn: a,
  url: u,
  ...c
}) {
  let p;
  const d = a ?? ((k) => new Promise(($) => setTimeout($, k)));
  return { stream: async function* () {
    let k = o ?? 3e3, $ = 0;
    const I = c.signal ?? new AbortController().signal;
    for (; !I.aborted; ) {
      $++;
      const P = c.headers instanceof Headers ? c.headers : new Headers(c.headers);
      p !== void 0 && P.set("Last-Event-ID", p);
      try {
        const R = {
          redirect: "follow",
          ...c,
          body: c.serializedBody,
          headers: P,
          signal: I
        };
        let B = new Request(u, R);
        e && (B = await e(u, R));
        const M = await (c.fetch ?? globalThis.fetch)(B);
        if (!M.ok) throw new Error(`SSE failed: ${M.status} ${M.statusText}`);
        if (!M.body) throw new Error("No body in SSE response");
        const q = M.body.pipeThrough(new TextDecoderStream()).getReader();
        let T = "";
        const te = () => {
          try {
            q.cancel();
          } catch {
          }
        };
        I.addEventListener("abort", te);
        try {
          for (; ; ) {
            const { done: re, value: de } = await q.read();
            if (re) break;
            T += de, T = T.replace(/\r\n?/g, `
`);
            const se = T.split(`

`);
            T = se.pop() ?? "";
            for (const E of se) {
              const C = E.split(`
`), O = [];
              let y;
              for (const v of C)
                if (v.startsWith("data:"))
                  O.push(v.replace(/^data:\s*/, ""));
                else if (v.startsWith("event:"))
                  y = v.replace(/^event:\s*/, "");
                else if (v.startsWith("id:"))
                  p = v.replace(/^id:\s*/, "");
                else if (v.startsWith("retry:")) {
                  const U = Number.parseInt(v.replace(/^retry:\s*/, ""), 10);
                  Number.isNaN(U) || (k = U);
                }
              let m, S = !1;
              if (O.length) {
                const v = O.join(`
`);
                try {
                  m = JSON.parse(v), S = !0;
                } catch {
                  m = v;
                }
              }
              S && (r && await r(m), s && (m = await s(m))), n == null || n({
                data: m,
                event: y,
                id: p,
                retry: k
              }), O.length && (yield m);
            }
          }
        } finally {
          I.removeEventListener("abort", te), q.releaseLock();
        }
        break;
      } catch (R) {
        if (t == null || t(R), l !== void 0 && $ >= l)
          break;
        const B = Math.min(k * 2 ** ($ - 1), i ?? 3e4);
        await d(B);
      }
    }
  }() };
}
const Dy = (e) => {
  switch (e) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, Py = (e) => {
  switch (e) {
    case "form":
      return ",";
    case "pipeDelimited":
      return "|";
    case "spaceDelimited":
      return "%20";
    default:
      return ",";
  }
}, Uy = (e) => {
  switch (e) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, Ls = ({
  allowReserved: e,
  explode: t,
  name: n,
  style: s,
  value: r
}) => {
  if (!t) {
    const i = (e ? r : r.map((a) => encodeURIComponent(a))).join(Py(s));
    switch (s) {
      case "label":
        return `.${i}`;
      case "matrix":
        return `;${n}=${i}`;
      case "simple":
        return i;
      default:
        return `${n}=${i}`;
    }
  }
  const o = Dy(s), l = r.map((i) => s === "label" || s === "simple" ? e ? i : encodeURIComponent(i) : gt({
    allowReserved: e,
    name: n,
    value: i
  })).join(o);
  return s === "label" || s === "matrix" ? o + l : l;
}, gt = ({
  allowReserved: e,
  name: t,
  value: n
}) => {
  if (n == null)
    return "";
  if (typeof n == "object")
    throw new Error(
      "Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these."
    );
  return `${t}=${e ? n : encodeURIComponent(n)}`;
}, Bs = ({
  allowReserved: e,
  explode: t,
  name: n,
  style: s,
  value: r,
  valueOnly: o
}) => {
  if (r instanceof Date)
    return o ? r.toISOString() : `${n}=${r.toISOString()}`;
  if (s !== "deepObject" && !t) {
    let a = [];
    Object.entries(r).forEach(([c, p]) => {
      a = [...a, c, e ? p : encodeURIComponent(p)];
    });
    const u = a.join(",");
    switch (s) {
      case "form":
        return `${n}=${u}`;
      case "label":
        return `.${u}`;
      case "matrix":
        return `;${n}=${u}`;
      default:
        return u;
    }
  }
  const l = Uy(s), i = Object.entries(r).map(
    ([a, u]) => gt({
      allowReserved: e,
      name: s === "deepObject" ? `${n}[${a}]` : a,
      value: u
    })
  ).join(l);
  return s === "label" || s === "matrix" ? l + i : i;
}, qy = /\{[^{}]+\}/g, jy = ({ path: e, url: t }) => {
  let n = t;
  const s = t.match(qy);
  if (s)
    for (const r of s) {
      let o = !1, l = r.substring(1, r.length - 1), i = "simple";
      l.endsWith("*") && (o = !0, l = l.substring(0, l.length - 1)), l.startsWith(".") ? (l = l.substring(1), i = "label") : l.startsWith(";") && (l = l.substring(1), i = "matrix");
      const a = e[l];
      if (a == null)
        continue;
      if (Array.isArray(a)) {
        n = n.replace(r, Ls({ explode: o, name: l, style: i, value: a }));
        continue;
      }
      if (typeof a == "object") {
        n = n.replace(
          r,
          Bs({
            explode: o,
            name: l,
            style: i,
            value: a,
            valueOnly: !0
          })
        );
        continue;
      }
      if (i === "matrix") {
        n = n.replace(
          r,
          `;${gt({
            name: l,
            value: a
          })}`
        );
        continue;
      }
      const u = encodeURIComponent(
        i === "label" ? `.${a}` : a
      );
      n = n.replace(r, u);
    }
  return n;
}, Hy = ({
  baseUrl: e,
  path: t,
  query: n,
  querySerializer: s,
  url: r
}) => {
  const o = r.startsWith("/") ? r : `/${r}`;
  let l = (e ?? "") + o;
  t && (l = jy({ path: t, url: l }));
  let i = n ? s(n) : "";
  return i.startsWith("?") && (i = i.substring(1)), i && (l += `?${i}`), l;
};
function Mn(e) {
  const t = e.body !== void 0;
  if (t && e.bodySerializer)
    return "serializedBody" in e ? e.serializedBody !== void 0 && e.serializedBody !== "" ? e.serializedBody : null : e.body !== "" ? e.body : null;
  if (t)
    return e.body;
}
const Vy = async (e, t) => {
  const n = typeof t == "function" ? await t(e) : t;
  if (n)
    return e.scheme === "bearer" ? `Bearer ${n}` : e.scheme === "basic" ? `Basic ${btoa(n)}` : n;
}, zs = ({
  parameters: e = {},
  ...t
} = {}) => (s) => {
  const r = [];
  if (s && typeof s == "object")
    for (const o in s) {
      const l = s[o];
      if (l == null)
        continue;
      const i = e[o] || t;
      if (Array.isArray(l)) {
        const a = Ls({
          allowReserved: i.allowReserved,
          explode: !0,
          name: o,
          style: "form",
          value: l,
          ...i.array
        });
        a && r.push(a);
      } else if (typeof l == "object") {
        const a = Bs({
          allowReserved: i.allowReserved,
          explode: !0,
          name: o,
          style: "deepObject",
          value: l,
          ...i.object
        });
        a && r.push(a);
      } else {
        const a = gt({
          allowReserved: i.allowReserved,
          name: o,
          value: l
        });
        a && r.push(a);
      }
    }
  return r.join("&");
}, Fy = (e) => {
  var n;
  if (!e)
    return "stream";
  const t = (n = e.split(";")[0]) == null ? void 0 : n.trim();
  if (t) {
    if (t.startsWith("application/json") || t.endsWith("+json"))
      return "json";
    if (t === "multipart/form-data")
      return "formData";
    if (["application/", "audio/", "image/", "video/"].some((s) => t.startsWith(s)))
      return "blob";
    if (t.startsWith("text/"))
      return "text";
  }
}, Gy = (e, t) => {
  var n, s;
  return t ? !!(e.headers.has(t) || (n = e.query) != null && n[t] || (s = e.headers.get("Cookie")) != null && s.includes(`${t}=`)) : !1;
};
async function Wy(e) {
  for (const t of e.security ?? []) {
    if (Gy(e, t.name))
      continue;
    const n = await Vy(t, e.auth);
    if (!n)
      continue;
    const s = t.name ?? "Authorization";
    switch (t.in) {
      case "query":
        e.query || (e.query = {}), e.query[s] = n;
        break;
      case "cookie":
        e.headers.append("Cookie", `${s}=${n}`);
        break;
      case "header":
      default:
        e.headers.set(s, n);
        break;
    }
  }
}
const An = (e) => Hy({
  baseUrl: e.baseUrl,
  path: e.path,
  query: e.query,
  querySerializer: typeof e.querySerializer == "function" ? e.querySerializer : zs(e.querySerializer),
  url: e.url
}), Cn = (e, t) => {
  var s;
  const n = { ...e, ...t };
  return (s = n.baseUrl) != null && s.endsWith("/") && (n.baseUrl = n.baseUrl.substring(0, n.baseUrl.length - 1)), n.headers = Ds(e.headers, t.headers), n;
}, Ky = (e) => {
  const t = [];
  return e.forEach((n, s) => {
    t.push([s, n]);
  }), t;
}, Ds = (...e) => {
  const t = new Headers();
  for (const n of e) {
    if (!n)
      continue;
    const s = n instanceof Headers ? Ky(n) : Object.entries(n);
    for (const [r, o] of s)
      if (o === null)
        t.delete(r);
      else if (Array.isArray(o))
        for (const l of o)
          t.append(r, l);
      else o !== void 0 && t.set(
        r,
        typeof o == "object" ? JSON.stringify(o) : o
      );
  }
  return t;
};
class vt {
  constructor() {
    G(this, "fns", []);
  }
  clear() {
    this.fns = [];
  }
  eject(t) {
    const n = this.getInterceptorIndex(t);
    this.fns[n] && (this.fns[n] = null);
  }
  exists(t) {
    const n = this.getInterceptorIndex(t);
    return !!this.fns[n];
  }
  getInterceptorIndex(t) {
    return typeof t == "number" ? this.fns[t] ? t : -1 : this.fns.indexOf(t);
  }
  update(t, n) {
    const s = this.getInterceptorIndex(t);
    return this.fns[s] ? (this.fns[s] = n, t) : !1;
  }
  use(t) {
    return this.fns.push(t), this.fns.length - 1;
  }
}
const Zy = () => ({
  error: new vt(),
  request: new vt(),
  response: new vt()
}), Xy = zs({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), Yy = {
  "Content-Type": "application/json"
}, Xt = (e = {}) => ({
  ...By,
  headers: Yy,
  parseAs: "auto",
  querySerializer: Xy,
  ...e
}), Ps = (e = {}) => {
  let t = Cn(Xt(), e);
  const n = () => ({ ...t }), s = (c) => (t = Cn(t, c), n()), r = Zy(), o = async (c) => {
    const p = {
      ...t,
      ...c,
      fetch: c.fetch ?? t.fetch ?? globalThis.fetch,
      headers: Ds(t.headers, c.headers),
      serializedBody: void 0
    };
    p.security && await Wy(p), p.requestValidator && await p.requestValidator(p), p.body !== void 0 && p.bodySerializer && (p.serializedBody = p.bodySerializer(p.body)), (p.body === void 0 || p.serializedBody === "") && p.headers.delete("Content-Type");
    const d = p, _ = An(d);
    return { opts: d, url: _ };
  }, l = async (c) => {
    const p = c.throwOnError ?? t.throwOnError, d = c.responseStyle ?? t.responseStyle;
    let _, f;
    try {
      const { opts: k, url: $ } = await o(c), I = {
        redirect: "follow",
        ...k,
        body: Mn(k)
      };
      _ = new Request($, I);
      for (const M of r.request.fns)
        M && (_ = await M(_, k));
      const P = k.fetch;
      f = await P(_);
      for (const M of r.response.fns)
        M && (f = await M(f, _, k));
      const R = {
        request: _,
        response: f
      };
      if (f.ok) {
        const M = (k.parseAs === "auto" ? Fy(f.headers.get("Content-Type")) : k.parseAs) ?? "json";
        if (f.status === 204 || f.headers.get("Content-Length") === "0") {
          let T;
          switch (M) {
            case "arrayBuffer":
            case "blob":
            case "text":
              T = await f[M]();
              break;
            case "formData":
              T = new FormData();
              break;
            case "stream":
              T = f.body;
              break;
            case "json":
            default:
              T = {};
              break;
          }
          return k.responseStyle === "data" ? T : {
            data: T,
            ...R
          };
        }
        let q;
        switch (M) {
          case "arrayBuffer":
          case "blob":
          case "formData":
          case "text":
            q = await f[M]();
            break;
          case "json": {
            const T = await f.text();
            q = T ? JSON.parse(T) : {};
            break;
          }
          case "stream":
            return k.responseStyle === "data" ? f.body : {
              data: f.body,
              ...R
            };
        }
        return M === "json" && (k.responseValidator && await k.responseValidator(q), k.responseTransformer && (q = await k.responseTransformer(q))), k.responseStyle === "data" ? q : {
          data: q,
          ...R
        };
      }
      const B = await f.text();
      let F;
      try {
        F = JSON.parse(B);
      } catch {
      }
      throw F ?? B;
    } catch (k) {
      let $ = k;
      for (const I of r.error.fns)
        I && ($ = await I($, f, _, c));
      if ($ = $ || {}, p)
        throw $;
      return d === "data" ? void 0 : {
        error: $,
        request: _,
        response: f
      };
    }
  }, i = (c) => (p) => l({ ...p, method: c }), a = (c) => async (p) => {
    const { opts: d, url: _ } = await o(p);
    return zy({
      ...d,
      body: d.body,
      method: c,
      onRequest: async (f, k) => {
        let $ = new Request(f, k);
        for (const I of r.request.fns)
          I && ($ = await I($, d));
        return $;
      },
      serializedBody: Mn(d),
      url: _
    });
  };
  return {
    buildUrl: (c) => An({ ...t, ...c }),
    connect: i("CONNECT"),
    delete: i("DELETE"),
    get: i("GET"),
    getConfig: n,
    head: i("HEAD"),
    interceptors: r,
    options: i("OPTIONS"),
    patch: i("PATCH"),
    post: i("POST"),
    put: i("PUT"),
    request: l,
    setConfig: s,
    sse: {
      connect: a("CONNECT"),
      delete: a("DELETE"),
      get: a("GET"),
      head: a("HEAD"),
      options: a("OPTIONS"),
      patch: a("PATCH"),
      post: a("POST"),
      put: a("PUT"),
      trace: a("TRACE")
    },
    trace: i("TRACE")
  };
}, K = Ps(Xt()), Jy = (e) => ((e == null ? void 0 : e.client) ?? K).get({ url: "/catchers", ...e }), Qy = (e) => ((e == null ? void 0 : e.client) ?? K).post({
  url: "/catchers",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e == null ? void 0 : e.headers
  }
}), eb = (e) => ((e == null ? void 0 : e.client) ?? K).get({ url: "/catchers/tools", ...e }), tb = (e) => ((e == null ? void 0 : e.client) ?? K).get({ url: "/catchers/read-tools", ...e }), nb = (e) => (e.client ?? K).delete({ url: "/catchers/{id}", ...e }), sb = (e) => (e.client ?? K).get({ url: "/catchers/{id}", ...e }), rb = (e) => (e.client ?? K).put({
  url: "/catchers/{id}",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), ab = (e) => (e.client ?? K).get({ url: "/catchers/{id}/versions", ...e }), ob = (e) => (e.client ?? K).patch({
  url: "/catchers/{id}/enabled",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), ib = (e) => (e.client ?? K).post({ url: "/catchers/{id}/consolidate", ...e }), lb = (e) => (e.client ?? K).post({
  url: "/catchers/{id}/save-as-template",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), cb = (e) => (e.client ?? K).post({ url: "/catchers/{id}/detach-template", ...e }), db = (e) => ((e == null ? void 0 : e.client) ?? K).get({ url: "/conversations", ...e }), ub = (e) => (e.client ?? K).get({ url: "/conversations/{id}/history", ...e }), hb = (e) => (e.client ?? K).get({ url: "/conversations/{id}", ...e }), pb = (e) => (e.client ?? K).get({ url: "/catchers/{catcherId}/facts", ...e }), fb = (e) => (e.client ?? K).delete({ url: "/facts/{id}", ...e }), gb = (e) => (e.client ?? K).put({
  url: "/facts/{id}",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), yb = (e) => ((e == null ? void 0 : e.client) ?? K).post({
  url: "/ingest/task-completed",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e == null ? void 0 : e.headers
  }
}), bb = (e) => ((e == null ? void 0 : e.client) ?? K).post({
  url: "/memory/recall",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e == null ? void 0 : e.headers
  }
}), mb = (e) => ((e == null ? void 0 : e.client) ?? K).post({
  url: "/memory/save",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e == null ? void 0 : e.headers
  }
}), kb = (e) => ((e == null ? void 0 : e.client) ?? K).post({
  url: "/memory/read",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e == null ? void 0 : e.headers
  }
}), _b = (e) => ((e == null ? void 0 : e.client) ?? K).get({ url: "/providers", ...e }), wb = (e) => (e.client ?? K).get({ url: "/tasks/{id}", ...e }), $b = (e) => ((e == null ? void 0 : e.client) ?? K).get({ url: "/templates", ...e }), vb = (e) => (e.client ?? K).delete({ url: "/templates/{id}", ...e }), Eb = (e) => (e.client ?? K).get({ url: "/templates/{id}", ...e }), Sb = (e) => (e.client ?? K).get({ url: "/templates/{id}/versions", ...e }), Tb = (e) => (e.client ?? K).post({
  url: "/templates/{id}/versions/{version}/instantiate",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), xb = (e) => ((e == null ? void 0 : e.client) ?? K).get({ url: "/traces", ...e }), Mb = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  deleteCatchersById: nb,
  deleteFactsById: fb,
  deleteTemplatesById: vb,
  getCatchers: Jy,
  getCatchersByCatcherIdFacts: pb,
  getCatchersById: sb,
  getCatchersByIdVersions: ab,
  getCatchersReadTools: tb,
  getCatchersTools: eb,
  getConversations: db,
  getConversationsById: hb,
  getConversationsByIdHistory: ub,
  getProviders: _b,
  getTasksById: wb,
  getTemplates: $b,
  getTemplatesById: Eb,
  getTemplatesByIdVersions: Sb,
  getTraces: xb,
  patchCatchersByIdEnabled: ob,
  postCatchers: Qy,
  postCatchersByIdConsolidate: ib,
  postCatchersByIdDetachTemplate: cb,
  postCatchersByIdSaveAsTemplate: lb,
  postIngestTaskCompleted: yb,
  postMemoryRead: kb,
  postMemoryRecall: bb,
  postMemorySave: mb,
  postTemplatesByIdVersionsByVersionInstantiate: Tb,
  putCatchersById: rb,
  putFactsById: gb
}, Symbol.toStringTag, { value: "Module" })), Us = (e) => Wt(ly, As(Kt({ baseUrl: e }))), qs = (e) => Wt(Ly, Rs(Zt({ baseUrl: e }))), js = (e) => Wt(Mb, Ps(Xt({ baseUrl: e })));
function Em(e) {
  return {
    wraith: Us(e.wraith),
    obsidian: qs(e.obsidian),
    chronos: js(e.chronos)
  };
}
class Sm {
  constructor(t) {
    G(this, "wraith");
    this.wraith = Us(t);
  }
  // ── Tools ────────────────────────────────────────────
  getTools() {
    return X(this.wraith.getTools());
  }
  getAllTools() {
    return X(this.wraith.getToolsAll());
  }
  async rebuildTools() {
    await X(this.wraith.postToolsRebuild());
  }
  async enableTool(t) {
    await X(this.wraith.postToolsByNameEnable({ path: { name: t } }));
  }
  async disableTool(t) {
    await X(this.wraith.postToolsByNameDisable({ path: { name: t } }));
  }
  // ── Toolboxes ─────────────────────────────────────────
  getToolboxes() {
    return X(this.wraith.getToolboxes());
  }
  // Add a new MCP server toolbox. Server persists + rebuilds the registry.
  // The server echoes the persisted record, so we surface it as McpToolboxInput
  // (the generated response types it loosely as Record<string, unknown>).
  async addMcpToolbox(t) {
    return await X(this.wraith.postToolboxesMcp({ body: t }));
  }
  // Remove a user-added toolbox. Hardcoded entries can't be deleted.
  async deleteToolbox(t) {
    await X(this.wraith.deleteToolboxesById({ path: { id: t } }));
  }
}
class Tm {
  constructor(t) {
    G(this, "obsidian");
    this.obsidian = qs(t);
  }
  // ── System / models ──────────────────────────────────
  ping() {
    return X(this.obsidian.getPing());
  }
  // NB: the /models route reads a `?provider=` query but doesn't declare it in
  // its OpenAPI schema, so the generated client can't type it. Returns models
  // for the default provider until Obsidian adds a query schema.
  getModels() {
    return X(this.obsidian.getModels());
  }
  // ── Config ───────────────────────────────────────────
  getConfig() {
    return X(this.obsidian.getConfig());
  }
  async setConfig(t) {
    return X(this.obsidian.postConfig({ body: t }));
  }
  async resetConfig() {
    await X(this.obsidian.deleteConfig());
  }
  // ── AI ───────────────────────────────────────────────
  // Single-turn chat (no tool loop).
  chat(t) {
    return X(this.obsidian.postChat({ body: t }));
  }
  // HTTP agent loop (local testing; production uses the WS endpoint — see ws-protocol.ts).
  agent(t) {
    return X(this.obsidian.postAgent({ body: t }));
  }
  // ── Event hooks ──────────────────────────────────────
  listHooks() {
    return X(this.obsidian.getHooks());
  }
  subscribeHook(t) {
    return X(this.obsidian.postHooks({ body: t }));
  }
  async unsubscribeHook(t) {
    await X(this.obsidian.deleteHooksById({ path: { id: t } }));
  }
}
async function Ab(e, t) {
  var i;
  const n = new TextEncoder(), s = n.encode(e), r = n.encode(t), o = (i = globalThis.crypto) == null ? void 0 : i.subtle;
  let l;
  if (o) {
    const a = await o.importKey("raw", s, { name: "HMAC", hash: "SHA-256" }, !1, ["sign"]);
    l = new Uint8Array(await o.sign("HMAC", a, r));
  } else
    l = Cb(s, r);
  return "sha256=" + [...l].map((a) => a.toString(16).padStart(2, "0")).join("");
}
function Cb(e, t) {
  e.length > 64 && (e = Et(e));
  const n = new Uint8Array(64), s = new Uint8Array(64);
  for (let r = 0; r < 64; r++)
    n[r] = (e[r] ?? 0) ^ 54, s[r] = (e[r] ?? 0) ^ 92;
  return Et(In(s, Et(In(n, t))));
}
function In(e, t) {
  const n = new Uint8Array(e.length + t.length);
  return n.set(e), n.set(t, e.length), n;
}
const Ib = new Uint32Array([
  1116352408,
  1899447441,
  3049323471,
  3921009573,
  961987163,
  1508970993,
  2453635748,
  2870763221,
  3624381080,
  310598401,
  607225278,
  1426881987,
  1925078388,
  2162078206,
  2614888103,
  3248222580,
  3835390401,
  4022224774,
  264347078,
  604807628,
  770255983,
  1249150122,
  1555081692,
  1996064986,
  2554220882,
  2821834349,
  2952996808,
  3210313671,
  3336571891,
  3584528711,
  113926993,
  338241895,
  666307205,
  773529912,
  1294757372,
  1396182291,
  1695183700,
  1986661051,
  2177026350,
  2456956037,
  2730485921,
  2820302411,
  3259730800,
  3345764771,
  3516065817,
  3600352804,
  4094571909,
  275423344,
  430227734,
  506948616,
  659060556,
  883997877,
  958139571,
  1322822218,
  1537002063,
  1747873779,
  1955562222,
  2024104815,
  2227730452,
  2361852424,
  2428436474,
  2756734187,
  3204031479,
  3329325298
]);
function Et(e) {
  const t = e.length, n = new Uint8Array((t + 8 >> 6) + 1 << 6);
  n.set(e), n[t] = 128;
  const s = new DataView(n.buffer);
  s.setUint32(n.length - 8, Math.floor(t / 536870912)), s.setUint32(n.length - 4, t << 3 >>> 0);
  let r = 1779033703, o = 3144134277, l = 1013904242, i = 2773480762, a = 1359893119, u = 2600822924, c = 528734635, p = 1541459225;
  const d = new Uint32Array(64);
  for (let k = 0; k < n.length; k += 64) {
    for (let T = 0; T < 16; T++) d[T] = s.getUint32(k + T * 4);
    for (let T = 16; T < 64; T++) {
      const te = ve(d[T - 15], 7) ^ ve(d[T - 15], 18) ^ d[T - 15] >>> 3, re = ve(d[T - 2], 17) ^ ve(d[T - 2], 19) ^ d[T - 2] >>> 10;
      d[T] = d[T - 16] + te + d[T - 7] + re >>> 0;
    }
    let $ = r, I = o, P = l, R = i, B = a, F = u, M = c, q = p;
    for (let T = 0; T < 64; T++) {
      const te = ve(B, 6) ^ ve(B, 11) ^ ve(B, 25), re = B & F ^ ~B & M, de = q + te + re + Ib[T] + d[T] >>> 0, se = ve($, 2) ^ ve($, 13) ^ ve($, 22), E = $ & I ^ $ & P ^ I & P, C = se + E >>> 0;
      q = M, M = F, F = B, B = R + de >>> 0, R = P, P = I, I = $, $ = de + C >>> 0;
    }
    r = r + $ >>> 0, o = o + I >>> 0, l = l + P >>> 0, i = i + R >>> 0, a = a + B >>> 0, u = u + F >>> 0, c = c + M >>> 0, p = p + q >>> 0;
  }
  const _ = new Uint8Array(32), f = new DataView(_.buffer);
  return [r, o, l, i, a, u, c, p].forEach((k, $) => f.setUint32($ * 4, k)), _;
}
const ve = (e, t) => (e >>> t | e << 32 - t) >>> 0;
class xm {
  constructor(t = "") {
    this.baseUrl = t;
  }
  // -- health --------------------------------------------------------------
  ping() {
    return this.get("/ping");
  }
  // -- flows ---------------------------------------------------------------
  listFlows(t = {}) {
    const n = new URLSearchParams();
    t.limit !== void 0 && n.set("limit", String(t.limit)), t.offset !== void 0 && n.set("offset", String(t.offset)), t.includeInactive !== void 0 && n.set("includeInactive", String(t.includeInactive));
    const s = n.toString();
    return this.get(`/flows${s ? `?${s}` : ""}`);
  }
  getFlow(t) {
    return this.get(`/flows/${t}`);
  }
  createFlow(t) {
    return this.send("POST", "/flows", t);
  }
  updateFlow(t, n) {
    return this.send("PUT", `/flows/${t}`, n);
  }
  async deleteFlow(t) {
    const n = await fetch(`${this.baseUrl}/flows/${t}`, { method: "DELETE" });
    if (!n.ok && n.status !== 204) throw await this.toError(n);
  }
  /** Creates the demo flows on the server. Idempotent — already-named flows skipped. */
  seedFlows() {
    return this.send("POST", "/flows/seed", {});
  }
  // -- triggers ------------------------------------------------------------
  listTriggers(t = {}) {
    const n = new URLSearchParams();
    t.flowId !== void 0 && n.set("flowId", t.flowId), t.limit !== void 0 && n.set("limit", String(t.limit)), t.offset !== void 0 && n.set("offset", String(t.offset));
    const s = n.toString();
    return this.get(`/triggers${s ? `?${s}` : ""}`);
  }
  getTrigger(t) {
    return this.get(`/triggers/${t}`);
  }
  createTrigger(t) {
    return this.send("POST", "/triggers", t);
  }
  updateTrigger(t, n) {
    return this.send("PUT", `/triggers/${t}`, n);
  }
  async deleteTrigger(t) {
    const n = await fetch(`${this.baseUrl}/triggers/${t}`, { method: "DELETE" });
    if (!n.ok && n.status !== 204) throw await this.toError(n);
  }
  /** Issues a fresh signing secret and returns the updated trigger. */
  rotateSecret(t) {
    return this.send("POST", `/triggers/${t}/secret`, {});
  }
  listDeliveries(t, n = {}) {
    const s = new URLSearchParams();
    n.limit !== void 0 && s.set("limit", String(n.limit)), n.offset !== void 0 && s.set("offset", String(n.offset));
    const r = s.toString();
    return this.get(`/triggers/${t}/deliveries${r ? `?${r}` : ""}`);
  }
  // -- agents --------------------------------------------------------------
  listAgents(t = {}) {
    const n = new URLSearchParams();
    t.limit !== void 0 && n.set("limit", String(t.limit)), t.offset !== void 0 && n.set("offset", String(t.offset));
    const s = n.toString();
    return this.get(`/agents${s ? `?${s}` : ""}`);
  }
  getAgent(t) {
    return this.get(`/agents/${t}`);
  }
  createAgent(t) {
    return this.send("POST", "/agents", t);
  }
  updateAgent(t, n) {
    return this.send("PUT", `/agents/${t}`, n);
  }
  async deleteAgent(t) {
    const n = await fetch(`${this.baseUrl}/agents/${t}`, { method: "DELETE" });
    if (!n.ok && n.status !== 204) throw await this.toError(n);
  }
  // -- skills (procedural memory) ------------------------------------------
  listSkills(t = {}) {
    const n = new URLSearchParams();
    t.limit !== void 0 && n.set("limit", String(t.limit)), t.offset !== void 0 && n.set("offset", String(t.offset));
    const s = n.toString();
    return this.get(`/skills${s ? `?${s}` : ""}`);
  }
  getSkill(t) {
    return this.get(`/skills/${t}`);
  }
  createSkill(t) {
    return this.send("POST", "/skills", t);
  }
  updateSkill(t, n) {
    return this.send("PUT", `/skills/${t}`, n);
  }
  async deleteSkill(t) {
    const n = await fetch(`${this.baseUrl}/skills/${t}`, { method: "DELETE" });
    if (!n.ok && n.status !== 204) throw await this.toError(n);
  }
  /** Self-learning: have the LLM draft a skill card. Lands as a draft pending approval. */
  draftSkill(t) {
    return this.send("POST", "/skills/draft", t);
  }
  /** Promote a draft (agent/observer authored) to active. */
  approveSkill(t) {
    return this.send("POST", `/skills/${t}/approve`, {});
  }
  /** Reinforcement: record one use of a skill. */
  useSkill(t) {
    return this.send("POST", `/skills/${t}/use`, {});
  }
  // -- provider connections ------------------------------------------------
  // The Hive's central registry of LLM endpoints + credentials. Keys are stored
  // server-side; the API only ever returns a masked hint.
  listProviders() {
    return this.get("/providers");
  }
  getProvider(t) {
    return this.get(`/providers/${t}`);
  }
  createProvider(t) {
    return this.send("POST", "/providers", t);
  }
  updateProvider(t, n) {
    return this.send("PUT", `/providers/${t}`, n);
  }
  async deleteProvider(t) {
    const n = await fetch(`${this.baseUrl}/providers/${t}`, { method: "DELETE" });
    if (!n.ok && n.status !== 204) throw await this.toError(n);
  }
  /** Live model ids advertised by a saved connection's provider. */
  listProviderModels(t) {
    return this.get(`/providers/${t}/models`);
  }
  /** Probes an embed model's vector dimension on a connection (one /embeddings call). */
  probeEmbedDims(t, n) {
    return this.send("POST", `/providers/${t}/embed-dims`, { model: n });
  }
  // -- favourite models ----------------------------------------------------
  // The shared shortlist of starred models; each carries its connection.
  listFavorites() {
    return this.get("/favorites");
  }
  addFavorite(t, n) {
    return this.send("POST", "/favorites", { connectionId: t, modelId: n });
  }
  async deleteFavorite(t) {
    const n = await fetch(`${this.baseUrl}/favorites/${t}`, { method: "DELETE" });
    if (!n.ok && n.status !== 204) throw await this.toError(n);
  }
  /**
   * Model ids for an ad-hoc (unsaved) baseUrl + key, via Spine's server-side
   * `/models/fetch` proxy. Parses the OpenAI-compatible `{ data: [{ id }] }` shape
   * down to a sorted id list.
   */
  async fetchModelIds(t, n) {
    const s = await this.send(
      "POST",
      "/models/fetch",
      { baseUrl: t, apiKey: n || void 0 }
    );
    return ((s == null ? void 0 : s.data) ?? []).map((r) => typeof r == "string" ? r : r.id).filter((r) => !!r).sort((r, o) => r.localeCompare(o));
  }
  // -- runs / tasks --------------------------------------------------------
  listRuns(t = {}) {
    const n = new URLSearchParams();
    t.flowId !== void 0 && n.set("flowId", t.flowId), t.limit !== void 0 && n.set("limit", String(t.limit)), t.offset !== void 0 && n.set("offset", String(t.offset));
    const s = n.toString();
    return this.get(`/runs${s ? `?${s}` : ""}`);
  }
  getRun(t) {
    return this.get(`/runs/${t}`);
  }
  /** Starts a manual run of a flow (queued for the worker). */
  runFlow(t) {
    return this.send("POST", `/flows/${t}/run`, {});
  }
  /**
   * Calls a webhook trigger the way an external system would: signs the raw
   * JSON body with the trigger's secret (X-Hive-Signature) and POSTs it to
   * /hooks/{triggerId}. Returns the queued FlowRun — execution is always
   * async server-side, so poll getRun() until it settles. Include a stable
   * `sessionId` GUID in the body for conversation continuity across calls.
   *
   * SECURITY: this needs the trigger's raw signing secret in the browser,
   * which works because the Triggers API is admin-only today. If trigger
   * reads are ever opened to less-privileged users, the secret must be
   * dropped from that response and this signing moved server-side.
   */
  async invokeWebhook(t, n, s = "sync") {
    const r = JSON.stringify(n), o = await Ab(t.secret, r), l = await fetch(`${this.baseUrl}/hooks/${t.id}?mode=${s}`, {
      method: "POST",
      headers: { "Content-Type": "application/json", "X-Hive-Signature": o },
      body: r
    });
    if (!l.ok) throw await this.toError(l);
    return l.json();
  }
  // -- internals -----------------------------------------------------------
  async get(t) {
    const n = await fetch(`${this.baseUrl}${t}`, { headers: { accept: "application/json" } });
    if (!n.ok) throw await this.toError(n);
    return n.json();
  }
  async send(t, n, s) {
    const r = await fetch(`${this.baseUrl}${n}`, {
      method: t,
      headers: { "content-type": "application/json", accept: "application/json" },
      body: JSON.stringify(s)
    });
    if (!r.ok) throw await this.toError(r);
    return r.json();
  }
  async toError(t) {
    var r;
    let n = `${t.status} ${t.statusText}`;
    try {
      const o = await t.json();
      (r = o.errors) != null && r.length && (n = o.errors.join("; "));
    } catch {
    }
    const s = new Error(n);
    return s.status = t.status, s;
  }
}
class Mm {
  constructor(t = "") {
    // Generated, fully-typed SDK (REST) — used for the newer endpoints. The
    // hand-written fetch/gql helpers below still serve the original methods +
    // GraphQL (which has no generated client).
    G(this, "client");
    this.baseUrl = t, this.client = js(t);
  }
  // ═══ REST ════════════════════════════════════════════
  // -- providers -----------------------------------------------------------
  listProviders() {
    return this.get("/providers");
  }
  // -- catchers (memory banks) ---------------------------------------------
  listCatchers() {
    return this.get("/catchers");
  }
  getCatcher(t) {
    return this.get(`/catchers/${t}`);
  }
  createCatcher(t) {
    return this.send("POST", "/catchers", t);
  }
  /** Full update; bumps the catcher's version. Empty providerApiKey keeps the current key. */
  updateCatcher(t, n) {
    return this.send("PUT", `/catchers/${t}`, n);
  }
  setEnabled(t, n) {
    return this.send("PATCH", `/catchers/${t}/enabled`, { enabled: n });
  }
  async deleteCatcher(t) {
    const n = await fetch(`${this.baseUrl}/catchers/${t}`, { method: "DELETE" });
    if (!n.ok && n.status !== 204) throw await this.toError(n);
  }
  // -- consolidation -------------------------------------------------------
  /** Queue a consolidation (dedupe/merge/prune) run for one bank. Returns immediately (202). */
  async consolidate(t) {
    await X(this.client.postCatchersByIdConsolidate({ path: { id: t } }));
  }
  /** List recorded consolidation runs for one bank, newest first. */
  listConsolidationRuns(t, n = 1, s = 20) {
    return this.get(
      `/catchers/${t}/consolidation-runs?page=${n}&pageSize=${s}`
    );
  }
  /** Queue a re-embed: re-projects BM25 facts whose vectors are missing from Qdrant. Returns immediately (202). */
  async reembed(t) {
    const n = await fetch(`${this.baseUrl}/catchers/${t}/reembed`, { method: "POST" });
    if (!n.ok && n.status !== 202) throw await this.toError(n);
  }
  // -- facts (the stored memories) -----------------------------------------
  async listFacts(t, n = {}) {
    const s = {};
    return n.access && (s.access = n.access), n.search && (s.search = n.search), n.page && (s.page = n.page), n.pageSize && (s.pageSize = n.pageSize), await X(this.client.getCatchersByCatcherIdFacts({ path: { catcherId: t }, query: s }));
  }
  async updateFact(t, n) {
    return await X(this.client.putFactsById({ path: { id: t }, body: { content: n } }));
  }
  async deleteFact(t) {
    await X(this.client.deleteFactsById({ path: { id: t } }));
  }
  // -- templates (git-like catcher config) ---------------------------------
  async listTemplates() {
    return await X(this.client.getTemplates());
  }
  async getTemplate(t) {
    return await X(this.client.getTemplatesById({ path: { id: t } }));
  }
  async listTemplateVersions(t) {
    return await X(this.client.getTemplatesByIdVersions({ path: { id: t } }));
  }
  async deleteTemplate(t) {
    await X(this.client.deleteTemplatesById({ path: { id: t } }));
  }
  /** Save a bank's current config as a new template (v1) and pin the bank to it. */
  async saveAsTemplate(t, n) {
    return await X(this.client.postCatchersByIdSaveAsTemplate({ path: { id: t }, body: n }));
  }
  /** Create a new bank from a template version, pinned to it. */
  async instantiateTemplate(t, n, s) {
    return await X(this.client.postTemplatesByIdVersionsByVersionInstantiate({
      path: { id: t, version: n },
      body: { name: s }
    }));
  }
  /** Clear a bank's template pin — future edits stop forking versions. */
  async detachTemplate(t) {
    return await X(this.client.postCatchersByIdDetachTemplate({ path: { id: t } }));
  }
  // -- recall --------------------------------------------------------------
  recall(t) {
    return this.send("POST", "/memory/recall", t);
  }
  // -- traces --------------------------------------------------------------
  listTraces(t = {}) {
    const n = new URLSearchParams();
    t.direction !== void 0 && n.set("direction", t.direction), t.catcherId !== void 0 && n.set("catcherId", t.catcherId), t.errorOnly !== void 0 && n.set("errorOnly", String(t.errorOnly)), t.limit !== void 0 && n.set("limit", String(t.limit));
    const s = n.toString();
    return this.get(`/traces${s ? `?${s}` : ""}`);
  }
  // ═══ GraphQL (read-only backtracking) ════════════════
  // -- sessions / conversations ----------------------------------------------
  async listConversations(t = {}) {
    var s, r;
    const n = await this.gql(
      `query ($take: Int, $skip: Int) {
        conversations(take: $take, skip: $skip, order: [{ createdAt: DESC }]) {
          totalCount
          items { id userId sessionId createdAt }
        }
      }`,
      { take: t.limit ?? 50, skip: t.offset ?? 0 }
    );
    return { items: ((s = n.conversations) == null ? void 0 : s.items) ?? [], totalCount: ((r = n.conversations) == null ? void 0 : r.totalCount) ?? 0 };
  }
  /** Full ordered message log of one conversation (tool calls excluded — see MessageRecordType). */
  getMessages(t, n = 500) {
    return this.items(
      `query ($id: UUID!, $take: Int) {
        messages(
          take: $take,
          where: { conversationId: { eq: $id } },
          order: [{ createdAt: ASC }, { messageIndex: ASC }]
        ) {
          items { id conversationId taskId messageIndex role content thoughts actions toolCallId createdAt }
        }
      }`,
      { id: t, take: n },
      "messages"
    );
  }
  /** The agent runs (tasks) referenced by a conversation's messages. */
  getTasksByIds(t) {
    return t.length === 0 ? Promise.resolve([]) : this.items(
      `query ($ids: [UUID!], $take: Int) {
        preformedTasks(take: $take, where: { id: { in: $ids } }, order: [{ startedAt: ASC }]) {
          items { id name userQuestion finalAnswer provider model inputTokens outputTokens startedAt finishedAt }
        }
      }`,
      { ids: t, take: t.length },
      "preformedTasks"
    );
  }
  /** Every memory read/write recorded for a session, oldest first. */
  getTracesBySession(t, n = 200) {
    return this.items(
      `query ($sid: String, $take: Int) {
        memoryCatcherTraces(
          take: $take,
          where: { sessionId: { eq: $sid } },
          order: [{ createdAt: ASC }]
        ) {
          items { id catcherId catcherVersion direction accessKey taskId sessionId userId matchedCount topScore durationMs error createdAt }
        }
      }`,
      { sid: t, take: n },
      "memoryCatcherTraces"
    );
  }
  // ═══ Plumbing ════════════════════════════════════════
  async get(t) {
    const n = await fetch(`${this.baseUrl}${t}`, { headers: { accept: "application/json" } });
    if (!n.ok) throw await this.toError(n);
    return n.json();
  }
  async send(t, n, s) {
    const r = await fetch(`${this.baseUrl}${n}`, {
      method: t,
      headers: { "content-type": "application/json", accept: "application/json" },
      body: JSON.stringify(s)
    });
    if (!r.ok) throw await this.toError(r);
    return r.json();
  }
  async toError(t) {
    var r;
    let n = `${t.status} ${t.statusText}`;
    try {
      const o = await t.json();
      (r = o.errors) != null && r.length ? n = o.errors.join("; ") : o.error ? n = o.error : o.message && (n = o.message);
    } catch {
    }
    const s = new Error(n);
    return s.status = t.status, s;
  }
  async items(t, n, s) {
    var o;
    return ((o = (await this.gql(t, n))[s]) == null ? void 0 : o.items) ?? [];
  }
  async gql(t, n) {
    var o;
    const s = await fetch(`${this.baseUrl}/graphql`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ query: t, variables: n })
    });
    if (!s.ok) throw new Error(`Chronos GraphQL: HTTP ${s.status} ${s.statusText}`);
    const r = await s.json();
    if ((o = r.errors) != null && o.length) throw new Error(r.errors.map((l) => l.message).join("; "));
    if (r.data === void 0 || r.data === null) throw new Error("Chronos GraphQL: empty response");
    return r.data;
  }
}
const Am = (e) => e.type === "tool_call", Cm = (e) => e.type === "thought", Im = (e) => e.type === "finished", Om = (e) => e.type === "failed";
export {
  Nn as Badge,
  Lb as Box,
  Bb as Button,
  fo as Card,
  zb as Checkbox,
  Mm as ChronosService,
  em as CodeBlock,
  km as Collapsible,
  om as ConfirmDialog,
  um as CopyButton,
  jb as Divider,
  im as Drawer,
  bi as EmptyState,
  Jb as FilterPills,
  _m as FlowCanvas,
  wn as FlowConnector,
  Wf as FlowNode,
  ms as Icon,
  qb as InfoLabel,
  pm as JsonViewer,
  gm as KeyValueList,
  ym as LoadingState,
  rm as Markdown,
  Uu as Modal,
  cm as ModelSelect,
  mn as NewBadge,
  wm as NodePalette,
  Tm as ObsidianService,
  Fb as PageHeader,
  Yb as ProgressBar,
  dm as SearchField,
  nm as Segmented,
  Ah as Select,
  Qb as SettingRow,
  Hb as SidebarNav,
  Pb as Slider,
  xm as SpineService,
  Yp as Spinner,
  Zb as Stat,
  Xb as StatStrip,
  hm as StatusBadge,
  Eo as StatusDot,
  bm as Tabs,
  Np as Tag,
  fm as TagInput,
  oa as TextField,
  tm as TextVarBlock,
  Ub as Textarea,
  Wb as Tile,
  Kb as TileGrid,
  lm as ToastHost,
  Db as Toggle,
  Gb as ToolRow,
  mm as Tooltip,
  Vb as TopBar,
  sm as VarChips,
  Sm as WraithService,
  js as chronosClient,
  Em as createHiveClient,
  Es as exceedsThreshold,
  am as iconNames,
  Om as isFailed,
  Im as isFinished,
  Cm as isThought,
  Am as isToolCall,
  ng as linearize,
  qs as obsidianClient,
  sg as useSortable,
  ih as useToast,
  Us as wraithClient
};

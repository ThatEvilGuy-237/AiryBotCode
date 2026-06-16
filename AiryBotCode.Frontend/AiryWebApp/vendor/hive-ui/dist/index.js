var tr = Object.defineProperty;
var nr = (e, t, n) => t in e ? tr(e, t, { enumerable: !0, configurable: !0, writable: !0, value: n }) : e[t] = n;
var V = (e, t, n) => nr(e, typeof t != "symbol" ? t + "" : t, n);
import { defineComponent as z, openBlock as g, createElementBlock as b, normalizeClass as h, renderSlot as Q, createElementVNode as $, createTextVNode as fe, toDisplayString as S, computed as Y, createCommentVNode as N, normalizeStyle as Se, Fragment as le, renderList as ye, createBlock as ce, unref as rt, withCtx as be, ref as re, h as Ct, resolveDynamicComponent as sr, watch as dt, onBeforeUnmount as Lt, Teleport as ut, withModifiers as zn, onMounted as rr, createVNode as Ae, resolveComponent as ar, withDirectives as or, withKeys as ir, vModelText as lr, Transition as cr } from "vue";
import { RouterLink as dr } from "vue-router";
const ur = /* @__PURE__ */ z({
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
}), hr = "_box_1640d_2", pr = "_bordered_1640d_5", fr = "_padded_1640d_9", gr = {
  box: hr,
  bordered: pr,
  padded: fr
}, P = (e, t) => {
  const n = e.__vccOpts || e;
  for (const [s, r] of t)
    n[s] = r;
  return n;
}, yr = {
  $style: gr
}, Kb = /* @__PURE__ */ P(ur, [["__cssModules", yr]]), br = ["type", "disabled"], mr = /* @__PURE__ */ z({
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
    ], 10, br));
  }
}), kr = "_btn_n5948_2", _r = "_sm_n5948_23", wr = "_md_n5948_29", $r = "_solid_n5948_36", vr = "_outline_n5948_45", Er = "_ghost_n5948_55", Sr = {
  btn: kr,
  sm: _r,
  md: wr,
  solid: $r,
  outline: vr,
  ghost: Er
}, Tr = {
  $style: Sr
}, Zb = /* @__PURE__ */ P(mr, [["__cssModules", Tr]]), xr = /* @__PURE__ */ z({
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
}), Mr = "_badge_11pzx_2", Cr = "_active_11pzx_14", Ar = "_inactive_11pzx_15", Ir = "_warning_11pzx_16", Or = "_danger_11pzx_17", Nr = {
  badge: Mr,
  active: Cr,
  inactive: Ar,
  warning: Ir,
  danger: Or
}, Rr = {
  $style: Nr
}, Pn = /* @__PURE__ */ P(xr, [["__cssModules", Rr]]), Lr = ["checked", "disabled"], Br = /* @__PURE__ */ z({
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
      $("input", {
        type: "checkbox",
        class: h(t.$style.input),
        checked: e.modelValue,
        disabled: e.disabled,
        onChange: n[0] || (n[0] = (s) => t.$emit("update:modelValue", s.target.checked))
      }, null, 42, Lr),
      fe(" " + S(e.label), 1)
    ], 2));
  }
}), zr = "_label_1b8zv_2", Pr = "_disabled_1b8zv_10", Dr = "_input_1b8zv_11", Ur = {
  label: zr,
  disabled: Pr,
  input: Dr
}, qr = {
  $style: Ur
}, Xb = /* @__PURE__ */ P(Br, [["__cssModules", qr]]), jr = ["aria-checked", "disabled"], Hr = /* @__PURE__ */ z({
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
    }, null, 10, jr));
  }
}), Vr = "_toggle_lyth5_2", Fr = "_on_lyth5_23", Gr = "_off_lyth5_28", Wr = {
  toggle: Vr,
  on: Fr,
  off: Gr
}, Kr = {
  $style: Wr
}, Yb = /* @__PURE__ */ P(Hr, [["__cssModules", Kr]]), Zr = ["min", "max", "step", "value", "disabled"], Xr = /* @__PURE__ */ z({
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
    ), a = Y(() => {
      const i = n.max - n.min;
      return i <= 0 ? 0 : Math.min(100, Math.max(0, (n.modelValue - n.min) / i * 100));
    });
    function l(i) {
      s("update:modelValue", Number(i.target.value));
    }
    return (i, o) => (g(), b("div", {
      class: h(i.$style.wrap)
    }, [
      e.label ? (g(), b("div", {
        key: 0,
        class: h(i.$style.head)
      }, [
        $("span", {
          class: h(i.$style.label)
        }, S(e.label), 3),
        $("span", {
          class: h(i.$style.value)
        }, S(r.value), 3)
      ], 2)) : N("", !0),
      $("input", {
        type: "range",
        class: h(i.$style.range),
        style: Se({ "--fill": a.value + "%" }),
        min: e.min,
        max: e.max,
        step: e.step,
        value: e.modelValue,
        disabled: e.disabled,
        onInput: l
      }, null, 46, Zr)
    ], 2));
  }
}), Yr = "_wrap_iz16t_2", Jr = "_head_iz16t_4", Qr = "_label_iz16t_5", ea = "_value_iz16t_6", ta = "_range_iz16t_14", na = {
  wrap: Yr,
  head: Jr,
  label: Qr,
  value: ea,
  range: ta
}, sa = {
  $style: na
}, Jb = /* @__PURE__ */ P(Xr, [["__cssModules", sa]]), ra = ["value", "placeholder"], aa = /* @__PURE__ */ z({
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
      }, S(e.label), 3)) : N("", !0),
      $("input", {
        type: "text",
        class: h(t.$style.input),
        value: e.modelValue,
        placeholder: e.placeholder,
        onInput: n[0] || (n[0] = (s) => t.$emit("update:modelValue", s.target.value))
      }, null, 42, ra)
    ], 2));
  }
}), oa = "_wrapper_q0661_2", ia = "_label_q0661_3", la = "_input_q0661_4", ca = {
  wrapper: oa,
  label: ia,
  input: la
}, da = {
  $style: ca
}, ua = /* @__PURE__ */ P(aa, [["__cssModules", da]]), ha = ["value", "placeholder", "rows"], pa = /* @__PURE__ */ z({
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
      }, S(e.label), 3)) : N("", !0),
      $("textarea", {
        class: h([t.$style.input, e.mono && t.$style.mono]),
        value: e.modelValue,
        placeholder: e.placeholder,
        rows: e.rows,
        onInput: n[0] || (n[0] = (s) => t.$emit("update:modelValue", s.target.value))
      }, null, 42, ha)
    ], 2));
  }
}), fa = "_wrapper_vb2o3_2", ga = "_label_vb2o3_3", ya = "_input_vb2o3_4", ba = "_mono_vb2o3_17", ma = {
  wrapper: fa,
  label: ga,
  input: ya,
  mono: ba
}, ka = {
  $style: ma
}, Qb = /* @__PURE__ */ P(pa, [["__cssModules", ka]]), _a = /* @__PURE__ */ z({
  __name: "InfoLabel",
  props: {
    label: {},
    value: {}
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.row)
    }, [
      $("span", {
        class: h(t.$style.label)
      }, S(e.label), 3),
      $("span", {
        class: h(t.$style.value)
      }, S(e.value), 3)
    ], 2));
  }
}), wa = "_row_1bo2k_2", $a = "_label_1bo2k_3", va = "_value_1bo2k_4", Ea = {
  row: wa,
  label: $a,
  value: va
}, Sa = {
  $style: Ea
}, em = /* @__PURE__ */ P(_a, [["__cssModules", Sa]]), Ta = /* @__PURE__ */ z({
  __name: "Divider",
  setup(e) {
    return (t, n) => (g(), b("hr", {
      class: h(t.$style.divider)
    }, null, 2));
  }
}), xa = "_divider_1obt9_2", Ma = {
  divider: xa
}, Ca = {
  $style: Ma
}, tm = /* @__PURE__ */ P(Ta, [["__cssModules", Ca]]), Aa = /* @__PURE__ */ z({
  __name: "SidebarNav",
  props: {
    items: {}
  },
  setup(e) {
    return (t, n) => (g(), b("nav", {
      class: h(t.$style.nav)
    }, [
      (g(!0), b(le, null, ye(e.items, (s) => (g(), ce(rt(dr), {
        key: s.href,
        to: s.href,
        class: h([t.$style.link, s.active && t.$style.active])
      }, {
        default: be(() => [
          fe(S(s.label), 1)
        ]),
        _: 2
      }, 1032, ["to", "class"]))), 128))
    ], 2));
  }
}), Ia = "_nav_1i6sa_2", Oa = "_link_1i6sa_13", Na = "_active_1i6sa_32", Ra = {
  nav: Ia,
  link: Oa,
  active: Na
}, La = {
  $style: Ra
}, nm = /* @__PURE__ */ P(Aa, [["__cssModules", La]]), Ba = /* @__PURE__ */ z({
  __name: "TopBar",
  props: {
    serviceName: {},
    status: {}
  },
  setup(e) {
    return (t, n) => (g(), b("header", {
      class: h(t.$style.bar)
    }, [
      $("div", {
        class: h(t.$style.left)
      }, [
        Q(t.$slots, "lead"),
        $("span", {
          class: h(t.$style.name)
        }, S(e.serviceName), 3)
      ], 2),
      e.status ? (g(), b("span", {
        key: 0,
        class: h([t.$style.status, t.$style[e.status]])
      }, [
        $("span", {
          class: h(t.$style.dot)
        }, null, 2),
        fe(" " + S(e.status), 1)
      ], 2)) : N("", !0)
    ], 2));
  }
}), za = "_bar_1i1qj_2", Pa = "_left_1i1qj_12", Da = "_name_1i1qj_19", Ua = "_status_1i1qj_27", qa = "_dot_1i1qj_38", ja = "_online_1i1qj_45", Ha = "_offline_1i1qj_46", Va = {
  bar: za,
  left: Pa,
  name: Da,
  status: Ua,
  dot: qa,
  online: ja,
  offline: Ha
}, Fa = {
  $style: Va
}, sm = /* @__PURE__ */ P(Ba, [["__cssModules", Fa]]), Ga = /* @__PURE__ */ z({
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
      $("div", {
        class: h(t.$style.titleBlock)
      }, [
        $("div", {
          class: h(t.$style.titleRow)
        }, [
          $("h1", {
            class: h(t.$style.title)
          }, S(e.title), 3),
          e.count !== void 0 ? (g(), ce(Pn, {
            key: 0,
            variant: "inactive"
          }, {
            default: be(() => [
              fe(S(e.count), 1)
            ]),
            _: 1
          })) : N("", !0)
        ], 2),
        e.subtitle ? (g(), b("p", {
          key: 0,
          class: h(t.$style.subtitle)
        }, S(e.subtitle), 3)) : N("", !0)
      ], 2),
      t.$slots.actions ? (g(), b("div", {
        key: 0,
        class: h(t.$style.actions)
      }, [
        Q(t.$slots, "actions")
      ], 2)) : N("", !0)
    ], 2));
  }
}), Wa = "_header_lwkhi_2", Ka = "_titleBlock_lwkhi_11", Za = "_titleRow_lwkhi_12", Xa = "_title_lwkhi_11", Ya = "_subtitle_lwkhi_15", Ja = "_actions_lwkhi_17", Qa = {
  header: Wa,
  titleBlock: Ka,
  titleRow: Za,
  title: Xa,
  subtitle: Ya,
  actions: Ja
}, eo = {
  $style: Qa
}, rm = /* @__PURE__ */ P(Ga, [["__cssModules", eo]]), to = /* @__PURE__ */ z({
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
      $("div", {
        class: h(t.$style.info)
      }, [
        $("span", {
          class: h(t.$style.name)
        }, S(e.name), 3),
        e.description ? (g(), b("span", {
          key: 0,
          class: h(t.$style.desc)
        }, S(e.description), 3)) : N("", !0)
      ], 2),
      t.$slots.actions ? (g(), b("div", {
        key: 0,
        class: h(t.$style.actions)
      }, [
        Q(t.$slots, "actions")
      ], 2)) : N("", !0)
    ], 2));
  }
}), no = "_row_rhqmk_2", so = "_info_rhqmk_10", ro = "_name_rhqmk_17", ao = "_desc_rhqmk_24", oo = "_actions_rhqmk_32", io = "_dim_rhqmk_34", lo = {
  row: no,
  info: so,
  name: ro,
  desc: ao,
  actions: oo,
  dim: io
}, co = {
  $style: lo
}, am = /* @__PURE__ */ P(to, [["__cssModules", co]]), uo = /* @__PURE__ */ z({
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
      ], 2)) : N("", !0),
      t.$slots.head && t.$slots.default ? (g(), b("hr", {
        key: 1,
        class: h(t.$style.divider)
      }, null, 2)) : N("", !0),
      t.$slots.default ? (g(), b("div", {
        key: 2,
        class: h(t.$style.body)
      }, [
        Q(t.$slots, "default")
      ], 2)) : N("", !0),
      t.$slots.default && t.$slots.foot ? (g(), b("hr", {
        key: 3,
        class: h(t.$style.divider)
      }, null, 2)) : N("", !0),
      t.$slots.foot ? (g(), b("div", {
        key: 4,
        class: h(t.$style.foot)
      }, [
        Q(t.$slots, "foot")
      ], 2)) : N("", !0)
    ], 2));
  }
}), ho = "_card_5x1u2_2", po = "_bordered_5x1u2_7", fo = "_head_5x1u2_11", go = "_body_5x1u2_17", yo = "_foot_5x1u2_23", bo = "_divider_5x1u2_29", mo = {
  card: ho,
  bordered: po,
  head: fo,
  body: go,
  foot: yo,
  divider: bo
}, ko = {
  $style: mo
}, _o = /* @__PURE__ */ P(uo, [["__cssModules", ko]]), wo = /* @__PURE__ */ z({
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
}), $o = "_dot_1t96t_2", vo = "_ok_1t96t_8", Eo = "_warn_1t96t_9", So = "_err_1t96t_10", To = "_off_1t96t_11", xo = "_live_1t96t_12", Mo = {
  dot: $o,
  ok: vo,
  warn: Eo,
  err: So,
  off: To,
  live: xo
}, Co = {
  $style: Mo
}, Ao = /* @__PURE__ */ P(wo, [["__cssModules", Co]]), Io = /* @__PURE__ */ z({
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
      $("div", {
        class: h(t.$style.label)
      }, S(e.label), 3),
      $("div", {
        class: h([t.$style.value, t.$style[e.variant]])
      }, S(e.value), 3),
      e.sub ? (g(), b("div", {
        key: 0,
        class: h([t.$style.sub, e.trend !== "none" && t.$style[e.trend]])
      }, S(e.sub), 3)) : N("", !0)
    ], 2));
  }
}), Oo = "_tile_1miuz_2", No = "_label_1miuz_12", Ro = "_value_1miuz_19", Lo = "_ok_1miuz_26", Bo = "_warn_1miuz_27", zo = "_err_1miuz_28", Po = "_sub_1miuz_29", Do = "_up_1miuz_34", Uo = "_down_1miuz_35", qo = {
  tile: Oo,
  label: No,
  value: Ro,
  ok: Lo,
  warn: Bo,
  err: zo,
  sub: Po,
  up: Do,
  down: Uo
}, jo = {
  $style: qo
}, om = /* @__PURE__ */ P(Io, [["__cssModules", jo]]), Ho = /* @__PURE__ */ z({
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
}), Vo = "_grid_77ada_2", Fo = {
  grid: Vo
}, Go = {
  $style: Fo
}, im = /* @__PURE__ */ P(Ho, [["__cssModules", Go]]), Wo = /* @__PURE__ */ z({
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
      $("div", {
        class: h(t.$style.label)
      }, S(e.label), 3),
      $("div", {
        class: h([t.$style.value, e.accent && t.$style.accent])
      }, S(e.value), 3),
      e.sub ? (g(), b("div", {
        key: 0,
        class: h(t.$style.sub)
      }, S(e.sub), 3)) : N("", !0)
    ], 2));
  }
}), Ko = "_stat_143op_2", Zo = "_label_143op_10", Xo = "_value_143op_17", Yo = "_accent_143op_24", Jo = "_sub_143op_25", Qo = {
  stat: Ko,
  label: Zo,
  value: Xo,
  accent: Yo,
  sub: Jo
}, ei = {
  $style: Qo
}, lm = /* @__PURE__ */ P(Wo, [["__cssModules", ei]]), ti = /* @__PURE__ */ z({
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
}), ni = "_strip_1f2y5_2", si = {
  strip: ni
}, ri = {
  $style: si
}, cm = /* @__PURE__ */ P(ti, [["__cssModules", ri]]), ai = ["aria-valuenow"], oi = /* @__PURE__ */ z({
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
      $("div", {
        class: h([s.$style.fill, s.$style[e.variant]]),
        style: Se({ width: `${n.value}%` })
      }, null, 6)
    ], 10, ai));
  }
}), ii = "_bar_thqsi_2", li = "_fill_thqsi_9", ci = "_accent_thqsi_12", di = "_ok_thqsi_13", ui = "_warn_thqsi_14", hi = "_err_thqsi_15", pi = {
  bar: ii,
  fill: li,
  accent: ci,
  ok: di,
  warn: ui,
  err: hi
}, fi = {
  $style: pi
}, dm = /* @__PURE__ */ P(oi, [["__cssModules", fi]]), gi = /* @__PURE__ */ z({
  __name: "EmptyState",
  props: {
    title: {},
    sub: {}
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.empty)
    }, [
      $("div", {
        class: h(t.$style.glyph)
      }, [
        Q(t.$slots, "glyph", {}, () => [
          n[0] || (n[0] = fe("+", -1))
        ])
      ], 2),
      $("div", {
        class: h(t.$style.title)
      }, S(e.title), 3),
      e.sub ? (g(), b("div", {
        key: 0,
        class: h(t.$style.sub)
      }, S(e.sub), 3)) : N("", !0),
      t.$slots.default ? (g(), b("div", {
        key: 1,
        class: h(t.$style.actions)
      }, [
        Q(t.$slots, "default")
      ], 2)) : N("", !0)
    ], 2));
  }
}), yi = "_empty_18kya_2", bi = "_glyph_18kya_14", mi = "_title_18kya_27", ki = "_sub_18kya_32", _i = "_actions_18kya_37", wi = {
  empty: yi,
  glyph: bi,
  title: mi,
  sub: ki,
  actions: _i
}, $i = {
  $style: wi
}, vi = /* @__PURE__ */ P(gi, [["__cssModules", $i]]), Ei = ["onClick"], Si = /* @__PURE__ */ z({
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
        fe(S(s.label) + " ", 1),
        s.count !== void 0 ? (g(), b("span", {
          key: 0,
          class: h(t.$style.count)
        }, S(s.count), 3)) : N("", !0)
      ], 10, Ei))), 128))
    ], 2));
  }
}), Ti = "_pills_hyvq3_2", xi = "_pill_hyvq3_2", Mi = "_on_hyvq3_27", Ci = "_count_hyvq3_33", Ai = {
  pills: Ti,
  pill: xi,
  on: Mi,
  count: Ci
}, Ii = {
  $style: Ai
}, um = /* @__PURE__ */ P(Si, [["__cssModules", Ii]]), Oi = /* @__PURE__ */ z({
  __name: "SettingRow",
  props: {
    name: {},
    description: {}
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.row)
    }, [
      $("div", {
        class: h(t.$style.text)
      }, [
        $("div", {
          class: h(t.$style.name)
        }, S(e.name), 3),
        e.description ? (g(), b("div", {
          key: 0,
          class: h(t.$style.desc)
        }, S(e.description), 3)) : N("", !0)
      ], 2),
      $("div", {
        class: h(t.$style.control)
      }, [
        Q(t.$slots, "default")
      ], 2)
    ], 2));
  }
}), Ni = "_row_bub7a_2", Ri = "_text_bub7a_8", Li = "_name_bub7a_15", Bi = "_desc_bub7a_20", zi = "_control_bub7a_24", Pi = {
  row: Ni,
  text: Ri,
  name: Li,
  desc: Bi,
  control: zi
}, Di = {
  $style: Pi
}, hm = /* @__PURE__ */ P(Oi, [["__cssModules", Di]]);
function Ui(e) {
  return e && e.__esModule && Object.prototype.hasOwnProperty.call(e, "default") ? e.default : e;
}
function Dn(e) {
  return e instanceof Map ? e.clear = e.delete = e.set = function() {
    throw new Error("map is read-only");
  } : e instanceof Set && (e.add = e.clear = e.delete = function() {
    throw new Error("set is read-only");
  }), Object.freeze(e), Object.getOwnPropertyNames(e).forEach((t) => {
    const n = e[t], s = typeof n;
    (s === "object" || s === "function") && !Object.isFrozen(n) && Dn(n);
  }), e;
}
class ln {
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
function Un(e) {
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
const qi = "</span>", cn = (e) => !!e.scope, ji = (e, { prefix: t }) => {
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
class Hi {
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
    this.buffer += Un(t);
  }
  /**
   * Adds a node open to the output stream (if needed)
   *
   * @param {Node} node */
  openNode(t) {
    if (!cn(t)) return;
    const n = ji(
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
    cn(t) && (this.buffer += qi);
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
const dn = (e = {}) => {
  const t = { children: [] };
  return Object.assign(t, e), t;
};
class Bt {
  constructor() {
    this.rootNode = dn(), this.stack = [this.rootNode];
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
    const n = dn({ scope: t });
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
      Bt._collapse(n);
    }));
  }
}
class Vi extends Bt {
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
    return new Hi(this, this.options).value();
  }
  finalize() {
    return this.closeAllNodes(), !0;
  }
}
function Ze(e) {
  return e ? typeof e == "string" ? e : e.source : null;
}
function qn(e) {
  return ze("(?=", e, ")");
}
function Fi(e) {
  return ze("(?:", e, ")*");
}
function Gi(e) {
  return ze("(?:", e, ")?");
}
function ze(...e) {
  return e.map((n) => Ze(n)).join("");
}
function Wi(e) {
  const t = e[e.length - 1];
  return typeof t == "object" && t.constructor === Object ? (e.splice(e.length - 1, 1), t) : {};
}
function zt(...e) {
  return "(" + (Wi(e).capture ? "" : "?:") + e.map((s) => Ze(s)).join("|") + ")";
}
function jn(e) {
  return new RegExp(e.toString() + "|").exec("").length - 1;
}
function Ki(e, t) {
  const n = e && e.exec(t);
  return n && n.index === 0;
}
const Zi = /\[(?:[^\\\]]|\\.)*\]|\(\??|\\([1-9][0-9]*)|\\./;
function Pt(e, { joinWith: t }) {
  let n = 0;
  return e.map((s) => {
    n += 1;
    const r = n;
    let a = Ze(s), l = "";
    for (; a.length > 0; ) {
      const i = Zi.exec(a);
      if (!i) {
        l += a;
        break;
      }
      l += a.substring(0, i.index), a = a.substring(i.index + i[0].length), i[0][0] === "\\" && i[1] ? l += "\\" + String(Number(i[1]) + r) : (l += i[0], i[0] === "(" && n++);
    }
    return l;
  }).map((s) => `(${s})`).join(t);
}
const Xi = /\b\B/, Hn = "[a-zA-Z]\\w*", Dt = "[a-zA-Z_]\\w*", Vn = "\\b\\d+(\\.\\d+)?", Fn = "(-?)(\\b0[xX][a-fA-F0-9]+|(\\b\\d+(\\.\\d*)?|\\.\\d+)([eE][-+]?\\d+)?)", Gn = "\\b(0b[01]+)", Yi = "!|!=|!==|%|%=|&|&&|&=|\\*|\\*=|\\+|\\+=|,|-|-=|/=|/|:|;|<<|<<=|<=|<|===|==|=|>>>=|>>=|>=|>>>|>>|>|\\?|\\[|\\{|\\(|\\^|\\^=|\\||\\|=|\\|\\||~", Ji = (e = {}) => {
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
}, Xe = {
  begin: "\\\\[\\s\\S]",
  relevance: 0
}, Qi = {
  scope: "string",
  begin: "'",
  end: "'",
  illegal: "\\n",
  contains: [Xe]
}, el = {
  scope: "string",
  begin: '"',
  end: '"',
  illegal: "\\n",
  contains: [Xe]
}, tl = {
  begin: /\b(a|an|the|are|I'm|isn't|don't|doesn't|won't|but|just|should|pretty|simply|enough|gonna|going|wtf|so|such|will|you|your|they|like|more)\b/
}, ht = function(e, t, n = {}) {
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
  const r = zt(
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
}, nl = ht("//", "$"), sl = ht("/\\*", "\\*/"), rl = ht("#", "$"), al = {
  scope: "number",
  begin: Vn,
  relevance: 0
}, ol = {
  scope: "number",
  begin: Fn,
  relevance: 0
}, il = {
  scope: "number",
  begin: Gn,
  relevance: 0
}, ll = {
  scope: "regexp",
  begin: /\/(?=[^/\n]*\/)/,
  end: /\/[gimuy]*/,
  contains: [
    Xe,
    {
      begin: /\[/,
      end: /\]/,
      relevance: 0,
      contains: [Xe]
    }
  ]
}, cl = {
  scope: "title",
  begin: Hn,
  relevance: 0
}, dl = {
  scope: "title",
  begin: Dt,
  relevance: 0
}, ul = {
  // excludes method names from keyword processing
  begin: "\\.\\s*" + Dt,
  relevance: 0
}, hl = function(e) {
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
var tt = /* @__PURE__ */ Object.freeze({
  __proto__: null,
  APOS_STRING_MODE: Qi,
  BACKSLASH_ESCAPE: Xe,
  BINARY_NUMBER_MODE: il,
  BINARY_NUMBER_RE: Gn,
  COMMENT: ht,
  C_BLOCK_COMMENT_MODE: sl,
  C_LINE_COMMENT_MODE: nl,
  C_NUMBER_MODE: ol,
  C_NUMBER_RE: Fn,
  END_SAME_AS_BEGIN: hl,
  HASH_COMMENT_MODE: rl,
  IDENT_RE: Hn,
  MATCH_NOTHING_RE: Xi,
  METHOD_GUARD: ul,
  NUMBER_MODE: al,
  NUMBER_RE: Vn,
  PHRASAL_WORDS_MODE: tl,
  QUOTE_STRING_MODE: el,
  REGEXP_MODE: ll,
  RE_STARTERS_RE: Yi,
  SHEBANG: Ji,
  TITLE_MODE: cl,
  UNDERSCORE_IDENT_RE: Dt,
  UNDERSCORE_TITLE_MODE: dl
});
function pl(e, t) {
  e.input[e.index - 1] === "." && t.ignoreMatch();
}
function fl(e, t) {
  e.className !== void 0 && (e.scope = e.className, delete e.className);
}
function gl(e, t) {
  t && e.beginKeywords && (e.begin = "\\b(" + e.beginKeywords.split(" ").join("|") + ")(?!\\.)(?=\\b|\\s)", e.__beforeBegin = pl, e.keywords = e.keywords || e.beginKeywords, delete e.beginKeywords, e.relevance === void 0 && (e.relevance = 0));
}
function yl(e, t) {
  Array.isArray(e.illegal) && (e.illegal = zt(...e.illegal));
}
function bl(e, t) {
  if (e.match) {
    if (e.begin || e.end) throw new Error("begin & end are not supported with match");
    e.begin = e.match, delete e.match;
  }
}
function ml(e, t) {
  e.relevance === void 0 && (e.relevance = 1);
}
const kl = (e, t) => {
  if (!e.beforeMatch) return;
  if (e.starts) throw new Error("beforeMatch cannot be used with starts");
  const n = Object.assign({}, e);
  Object.keys(e).forEach((s) => {
    delete e[s];
  }), e.keywords = n.keywords, e.begin = ze(n.beforeMatch, qn(n.begin)), e.starts = {
    relevance: 0,
    contains: [
      Object.assign(n, { endsParent: !0 })
    ]
  }, e.relevance = 0, delete n.beforeMatch;
}, _l = [
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
], wl = "keyword";
function Wn(e, t, n = wl) {
  const s = /* @__PURE__ */ Object.create(null);
  return typeof e == "string" ? r(n, e.split(" ")) : Array.isArray(e) ? r(n, e) : Object.keys(e).forEach(function(a) {
    Object.assign(
      s,
      Wn(e[a], t, a)
    );
  }), s;
  function r(a, l) {
    t && (l = l.map((i) => i.toLowerCase())), l.forEach(function(i) {
      const o = i.split("|");
      s[o[0]] = [a, $l(o[0], o[1])];
    });
  }
}
function $l(e, t) {
  return t ? Number(t) : vl(e) ? 0 : 1;
}
function vl(e) {
  return _l.includes(e.toLowerCase());
}
const un = {}, Re = (e) => {
  console.error(e);
}, hn = (e, ...t) => {
  console.log(`WARN: ${e}`, ...t);
}, De = (e, t) => {
  un[`${e}/${t}`] || (console.log(`Deprecated as of ${e}. ${t}`), un[`${e}/${t}`] = !0);
}, at = new Error();
function Kn(e, t, { key: n }) {
  let s = 0;
  const r = e[n], a = {}, l = {};
  for (let i = 1; i <= t.length; i++)
    l[i + s] = r[i], a[i + s] = !0, s += jn(t[i - 1]);
  e[n] = l, e[n]._emit = a, e[n]._multi = !0;
}
function El(e) {
  if (Array.isArray(e.begin)) {
    if (e.skip || e.excludeBegin || e.returnBegin)
      throw Re("skip, excludeBegin, returnBegin not compatible with beginScope: {}"), at;
    if (typeof e.beginScope != "object" || e.beginScope === null)
      throw Re("beginScope must be object"), at;
    Kn(e, e.begin, { key: "beginScope" }), e.begin = Pt(e.begin, { joinWith: "" });
  }
}
function Sl(e) {
  if (Array.isArray(e.end)) {
    if (e.skip || e.excludeEnd || e.returnEnd)
      throw Re("skip, excludeEnd, returnEnd not compatible with endScope: {}"), at;
    if (typeof e.endScope != "object" || e.endScope === null)
      throw Re("endScope must be object"), at;
    Kn(e, e.end, { key: "endScope" }), e.end = Pt(e.end, { joinWith: "" });
  }
}
function Tl(e) {
  e.scope && typeof e.scope == "object" && e.scope !== null && (e.beginScope = e.scope, delete e.scope);
}
function xl(e) {
  Tl(e), typeof e.beginScope == "string" && (e.beginScope = { _wrap: e.beginScope }), typeof e.endScope == "string" && (e.endScope = { _wrap: e.endScope }), El(e), Sl(e);
}
function Ml(e) {
  function t(l, i) {
    return new RegExp(
      Ze(l),
      "m" + (e.case_insensitive ? "i" : "") + (e.unicodeRegex ? "u" : "") + (i ? "g" : "")
    );
  }
  class n {
    constructor() {
      this.matchIndexes = {}, this.regexes = [], this.matchAt = 1, this.position = 0;
    }
    // @ts-ignore
    addRule(i, o) {
      o.position = this.position++, this.matchIndexes[this.matchAt] = o, this.regexes.push([o, i]), this.matchAt += jn(i) + 1;
    }
    compile() {
      this.regexes.length === 0 && (this.exec = () => null);
      const i = this.regexes.map((o) => o[1]);
      this.matcherRe = t(Pt(i, { joinWith: "|" }), !0), this.lastIndex = 0;
    }
    /** @param {string} s */
    exec(i) {
      this.matcherRe.lastIndex = this.lastIndex;
      const o = this.matcherRe.exec(i);
      if (!o)
        return null;
      const d = o.findIndex((p, u) => u > 0 && p !== void 0), c = this.matchIndexes[d];
      return o.splice(0, d), Object.assign(o, c);
    }
  }
  class s {
    constructor() {
      this.rules = [], this.multiRegexes = [], this.count = 0, this.lastIndex = 0, this.regexIndex = 0;
    }
    // @ts-ignore
    getMatcher(i) {
      if (this.multiRegexes[i]) return this.multiRegexes[i];
      const o = new n();
      return this.rules.slice(i).forEach(([d, c]) => o.addRule(d, c)), o.compile(), this.multiRegexes[i] = o, o;
    }
    resumingScanAtSamePosition() {
      return this.regexIndex !== 0;
    }
    considerAll() {
      this.regexIndex = 0;
    }
    // @ts-ignore
    addRule(i, o) {
      this.rules.push([i, o]), o.type === "begin" && this.count++;
    }
    /** @param {string} s */
    exec(i) {
      const o = this.getMatcher(this.regexIndex);
      o.lastIndex = this.lastIndex;
      let d = o.exec(i);
      if (this.resumingScanAtSamePosition() && !(d && d.index === this.lastIndex)) {
        const c = this.getMatcher(0);
        c.lastIndex = this.lastIndex + 1, d = c.exec(i);
      }
      return d && (this.regexIndex += d.position + 1, this.regexIndex === this.count && this.considerAll()), d;
    }
  }
  function r(l) {
    const i = new s();
    return l.contains.forEach((o) => i.addRule(o.begin, { rule: o, type: "begin" })), l.terminatorEnd && i.addRule(l.terminatorEnd, { type: "end" }), l.illegal && i.addRule(l.illegal, { type: "illegal" }), i;
  }
  function a(l, i) {
    const o = (
      /** @type CompiledMode */
      l
    );
    if (l.isCompiled) return o;
    [
      fl,
      // do this early so compiler extensions generally don't have to worry about
      // the distinction between match/begin
      bl,
      xl,
      kl
    ].forEach((c) => c(l, i)), e.compilerExtensions.forEach((c) => c(l, i)), l.__beforeBegin = null, [
      gl,
      // do this later so compiler extensions that come earlier have access to the
      // raw array if they wanted to perhaps manipulate it, etc.
      yl,
      // default to 1 relevance if not specified
      ml
    ].forEach((c) => c(l, i)), l.isCompiled = !0;
    let d = null;
    return typeof l.keywords == "object" && l.keywords.$pattern && (l.keywords = Object.assign({}, l.keywords), d = l.keywords.$pattern, delete l.keywords.$pattern), d = d || /\w+/, l.keywords && (l.keywords = Wn(l.keywords, e.case_insensitive)), o.keywordPatternRe = t(d, !0), i && (l.begin || (l.begin = /\B|\b/), o.beginRe = t(o.begin), !l.end && !l.endsWithParent && (l.end = /\B|\b/), l.end && (o.endRe = t(o.end)), o.terminatorEnd = Ze(o.end) || "", l.endsWithParent && i.terminatorEnd && (o.terminatorEnd += (l.end ? "|" : "") + i.terminatorEnd)), l.illegal && (o.illegalRe = t(
      /** @type {RegExp | string} */
      l.illegal
    )), l.contains || (l.contains = []), l.contains = [].concat(...l.contains.map(function(c) {
      return Cl(c === "self" ? l : c);
    })), l.contains.forEach(function(c) {
      a(
        /** @type Mode */
        c,
        o
      );
    }), l.starts && a(l.starts, i), o.matcher = r(o), o;
  }
  if (e.compilerExtensions || (e.compilerExtensions = []), e.contains && e.contains.includes("self"))
    throw new Error("ERR: contains `self` is not supported at the top-level of a language.  See documentation.");
  return e.classNameAliases = Ie(e.classNameAliases || {}), a(
    /** @type Mode */
    e
  );
}
function Zn(e) {
  return e ? e.endsWithParent || Zn(e.starts) : !1;
}
function Cl(e) {
  return e.variants && !e.cachedVariants && (e.cachedVariants = e.variants.map(function(t) {
    return Ie(e, { variants: null }, t);
  })), e.cachedVariants ? e.cachedVariants : Zn(e) ? Ie(e, { starts: e.starts ? Ie(e.starts) : null }) : Object.isFrozen(e) ? Ie(e) : e;
}
var Al = "11.11.1";
class Il extends Error {
  constructor(t, n) {
    super(t), this.name = "HTMLInjectionError", this.html = n;
  }
}
const wt = Un, pn = Ie, fn = Symbol("nomatch"), Ol = 7, Xn = function(e) {
  const t = /* @__PURE__ */ Object.create(null), n = /* @__PURE__ */ Object.create(null), s = [];
  let r = !0;
  const a = "Could not find the language '{}', did you forget to load/include a language module?", l = { disableAutodetect: !0, name: "Plain text", contains: [] };
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
    __emitter: Vi
  };
  function o(m) {
    return i.noHighlightRe.test(m);
  }
  function d(m) {
    let w = m.className + " ";
    w += m.parentNode ? m.parentNode.className : "";
    const T = i.languageDetectRe.exec(w);
    if (T) {
      const v = x(T[1]);
      return v || (hn(a.replace("{}", T[1])), hn("Falling back to no-highlight mode for this block.", m)), v ? T[1] : "no-highlight";
    }
    return w.split(/\s+/).find((v) => o(v) || x(v));
  }
  function c(m, w, T) {
    let v = "", U = "";
    typeof w == "object" ? (v = m, T = w.ignoreIllegals, U = w.language) : (De("10.7.0", "highlight(lang, code, ...args) has been deprecated."), De("10.7.0", `Please use highlight(code, options) instead.
https://github.com/highlightjs/highlight.js/issues/2277`), U = m, v = w), T === void 0 && (T = !0);
    const ne = {
      code: v,
      language: U
    };
    I("before:highlight", ne);
    const ue = ne.result ? ne.result : p(ne.language, ne.code, T);
    return ue.code = ne.code, I("after:highlight", ue), ue;
  }
  function p(m, w, T, v) {
    const U = /* @__PURE__ */ Object.create(null);
    function ne(A, R) {
      return A.keywords[R];
    }
    function ue() {
      if (!j.keywords) {
        ie.addText(ee);
        return;
      }
      let A = 0;
      j.keywordPatternRe.lastIndex = 0;
      let R = j.keywordPatternRe.exec(ee), F = "";
      for (; R; ) {
        F += ee.substring(A, R.index);
        const Z = $e.case_insensitive ? R[0].toLowerCase() : R[0], he = ne(j, Z);
        if (he) {
          const [Me, Qs] = he;
          if (ie.addText(F), F = "", U[Z] = (U[Z] || 0) + 1, U[Z] <= Ol && (et += Qs), Me.startsWith("_"))
            F += R[0];
          else {
            const er = $e.classNameAliases[Me] || Me;
            we(R[0], er);
          }
        } else
          F += R[0];
        A = j.keywordPatternRe.lastIndex, R = j.keywordPatternRe.exec(ee);
      }
      F += ee.substring(A), ie.addText(F);
    }
    function _e() {
      if (ee === "") return;
      let A = null;
      if (typeof j.subLanguage == "string") {
        if (!t[j.subLanguage]) {
          ie.addText(ee);
          return;
        }
        A = p(j.subLanguage, ee, !0, on[j.subLanguage]), on[j.subLanguage] = /** @type {CompiledMode} */
        A._top;
      } else
        A = _(ee, j.subLanguage.length ? j.subLanguage : null);
      j.relevance > 0 && (et += A.relevance), ie.__addSublanguage(A._emitter, A.language);
    }
    function ge() {
      j.subLanguage != null ? _e() : ue(), ee = "";
    }
    function we(A, R) {
      A !== "" && (ie.startScope(R), ie.addText(A), ie.endScope());
    }
    function nn(A, R) {
      let F = 1;
      const Z = R.length - 1;
      for (; F <= Z; ) {
        if (!A._emit[F]) {
          F++;
          continue;
        }
        const he = $e.classNameAliases[A[F]] || A[F], Me = R[F];
        he ? we(Me, he) : (ee = Me, ue(), ee = ""), F++;
      }
    }
    function sn(A, R) {
      return A.scope && typeof A.scope == "string" && ie.openNode($e.classNameAliases[A.scope] || A.scope), A.beginScope && (A.beginScope._wrap ? (we(ee, $e.classNameAliases[A.beginScope._wrap] || A.beginScope._wrap), ee = "") : A.beginScope._multi && (nn(A.beginScope, R), ee = "")), j = Object.create(A, { parent: { value: j } }), j;
    }
    function rn(A, R, F) {
      let Z = Ki(A.endRe, F);
      if (Z) {
        if (A["on:end"]) {
          const he = new ln(A);
          A["on:end"](R, he), he.isMatchIgnored && (Z = !1);
        }
        if (Z) {
          for (; A.endsParent && A.parent; )
            A = A.parent;
          return A;
        }
      }
      if (A.endsWithParent)
        return rn(A.parent, R, F);
    }
    function Ks(A) {
      return j.matcher.regexIndex === 0 ? (ee += A[0], 1) : (_t = !0, 0);
    }
    function Zs(A) {
      const R = A[0], F = A.rule, Z = new ln(F), he = [F.__beforeBegin, F["on:begin"]];
      for (const Me of he)
        if (Me && (Me(A, Z), Z.isMatchIgnored))
          return Ks(R);
      return F.skip ? ee += R : (F.excludeBegin && (ee += R), ge(), !F.returnBegin && !F.excludeBegin && (ee = R)), sn(F, A), F.returnBegin ? 0 : R.length;
    }
    function Xs(A) {
      const R = A[0], F = w.substring(A.index), Z = rn(j, A, F);
      if (!Z)
        return fn;
      const he = j;
      j.endScope && j.endScope._wrap ? (ge(), we(R, j.endScope._wrap)) : j.endScope && j.endScope._multi ? (ge(), nn(j.endScope, A)) : he.skip ? ee += R : (he.returnEnd || he.excludeEnd || (ee += R), ge(), he.excludeEnd && (ee = R));
      do
        j.scope && ie.closeNode(), !j.skip && !j.subLanguage && (et += j.relevance), j = j.parent;
      while (j !== Z.parent);
      return Z.starts && sn(Z.starts, A), he.returnEnd ? 0 : R.length;
    }
    function Ys() {
      const A = [];
      for (let R = j; R !== $e; R = R.parent)
        R.scope && A.unshift(R.scope);
      A.forEach((R) => ie.openNode(R));
    }
    let Qe = {};
    function an(A, R) {
      const F = R && R[0];
      if (ee += A, F == null)
        return ge(), 0;
      if (Qe.type === "begin" && R.type === "end" && Qe.index === R.index && F === "") {
        if (ee += w.slice(R.index, R.index + 1), !r) {
          const Z = new Error(`0 width match regex (${m})`);
          throw Z.languageName = m, Z.badRule = Qe.rule, Z;
        }
        return 1;
      }
      if (Qe = R, R.type === "begin")
        return Zs(R);
      if (R.type === "illegal" && !T) {
        const Z = new Error('Illegal lexeme "' + F + '" for mode "' + (j.scope || "<unnamed>") + '"');
        throw Z.mode = j, Z;
      } else if (R.type === "end") {
        const Z = Xs(R);
        if (Z !== fn)
          return Z;
      }
      if (R.type === "illegal" && F === "")
        return ee += `
`, 1;
      if (kt > 1e5 && kt > R.index * 3)
        throw new Error("potential infinite loop, way more iterations than matches");
      return ee += F, F.length;
    }
    const $e = x(m);
    if (!$e)
      throw Re(a.replace("{}", m)), new Error('Unknown language: "' + m + '"');
    const Js = Ml($e);
    let mt = "", j = v || Js;
    const on = {}, ie = new i.__emitter(i);
    Ys();
    let ee = "", et = 0, Oe = 0, kt = 0, _t = !1;
    try {
      if ($e.__emitTokens)
        $e.__emitTokens(w, ie);
      else {
        for (j.matcher.considerAll(); ; ) {
          kt++, _t ? _t = !1 : j.matcher.considerAll(), j.matcher.lastIndex = Oe;
          const A = j.matcher.exec(w);
          if (!A) break;
          const R = w.substring(Oe, A.index), F = an(R, A);
          Oe = A.index + F;
        }
        an(w.substring(Oe));
      }
      return ie.finalize(), mt = ie.toHTML(), {
        language: m,
        value: mt,
        relevance: et,
        illegal: !1,
        _emitter: ie,
        _top: j
      };
    } catch (A) {
      if (A.message && A.message.includes("Illegal"))
        return {
          language: m,
          value: wt(w),
          illegal: !0,
          relevance: 0,
          _illegalBy: {
            message: A.message,
            index: Oe,
            context: w.slice(Oe - 100, Oe + 100),
            mode: A.mode,
            resultSoFar: mt
          },
          _emitter: ie
        };
      if (r)
        return {
          language: m,
          value: wt(w),
          illegal: !1,
          relevance: 0,
          errorRaised: A,
          _emitter: ie,
          _top: j
        };
      throw A;
    }
  }
  function u(m) {
    const w = {
      value: wt(m),
      illegal: !1,
      relevance: 0,
      _top: l,
      _emitter: new i.__emitter(i)
    };
    return w._emitter.addText(m), w;
  }
  function _(m, w) {
    w = w || i.languages || Object.keys(t);
    const T = u(m), v = w.filter(x).filter(ae).map(
      (ge) => p(ge, m, !1)
    );
    v.unshift(T);
    const U = v.sort((ge, we) => {
      if (ge.relevance !== we.relevance) return we.relevance - ge.relevance;
      if (ge.language && we.language) {
        if (x(ge.language).supersetOf === we.language)
          return 1;
        if (x(we.language).supersetOf === ge.language)
          return -1;
      }
      return 0;
    }), [ne, ue] = U, _e = ne;
    return _e.secondBest = ue, _e;
  }
  function f(m, w, T) {
    const v = w && n[w] || T;
    m.classList.add("hljs"), m.classList.add(`language-${v}`);
  }
  function k(m) {
    let w = null;
    const T = d(m);
    if (o(T)) return;
    if (I(
      "before:highlightElement",
      { el: m, language: T }
    ), m.dataset.highlighted) {
      console.log("Element previously highlighted. To highlight again, first unset `dataset.highlighted`.", m);
      return;
    }
    if (m.children.length > 0 && (i.ignoreUnescapedHTML || (console.warn("One of your code blocks includes unescaped HTML. This is a potentially serious security risk."), console.warn("https://github.com/highlightjs/highlight.js/wiki/security"), console.warn("The element with unescaped HTML:"), console.warn(m)), i.throwUnescapedHTML))
      throw new Il(
        "One of your code blocks includes unescaped HTML.",
        m.innerHTML
      );
    w = m;
    const v = w.textContent, U = T ? c(v, { language: T, ignoreIllegals: !0 }) : _(v);
    m.innerHTML = U.value, m.dataset.highlighted = "yes", f(m, T, U.language), m.result = {
      language: U.language,
      // TODO: remove with version 11.0
      re: U.relevance,
      relevance: U.relevance
    }, U.secondBest && (m.secondBest = {
      language: U.secondBest.language,
      relevance: U.secondBest.relevance
    }), I("after:highlightElement", { el: m, result: U, text: v });
  }
  function y(m) {
    i = pn(i, m);
  }
  const C = () => {
    B(), De("10.6.0", "initHighlighting() deprecated.  Use highlightAll() now.");
  };
  function D() {
    B(), De("10.6.0", "initHighlightingOnLoad() deprecated.  Use highlightAll() now.");
  }
  let L = !1;
  function B() {
    function m() {
      B();
    }
    if (document.readyState === "loading") {
      L || window.addEventListener("DOMContentLoaded", m, !1), L = !0;
      return;
    }
    document.querySelectorAll(i.cssSelector).forEach(k);
  }
  function G(m, w) {
    let T = null;
    try {
      T = w(e);
    } catch (v) {
      if (Re("Language definition for '{}' could not be registered.".replace("{}", m)), r)
        Re(v);
      else
        throw v;
      T = l;
    }
    T.name || (T.name = m), t[m] = T, T.rawDefinition = w.bind(null, e), T.aliases && te(T.aliases, { languageName: m });
  }
  function M(m) {
    delete t[m];
    for (const w of Object.keys(n))
      n[w] === m && delete n[w];
  }
  function q() {
    return Object.keys(t);
  }
  function x(m) {
    return m = (m || "").toLowerCase(), t[m] || t[n[m]];
  }
  function te(m, { languageName: w }) {
    typeof m == "string" && (m = [m]), m.forEach((T) => {
      n[T.toLowerCase()] = w;
    });
  }
  function ae(m) {
    const w = x(m);
    return w && !w.disableAutodetect;
  }
  function de(m) {
    m["before:highlightBlock"] && !m["before:highlightElement"] && (m["before:highlightElement"] = (w) => {
      m["before:highlightBlock"](
        Object.assign({ block: w.el }, w)
      );
    }), m["after:highlightBlock"] && !m["after:highlightElement"] && (m["after:highlightElement"] = (w) => {
      m["after:highlightBlock"](
        Object.assign({ block: w.el }, w)
      );
    });
  }
  function se(m) {
    de(m), s.push(m);
  }
  function E(m) {
    const w = s.indexOf(m);
    w !== -1 && s.splice(w, 1);
  }
  function I(m, w) {
    const T = m;
    s.forEach(function(v) {
      v[T] && v[T](w);
    });
  }
  function O(m) {
    return De("10.7.0", "highlightBlock will be removed entirely in v12.0"), De("10.7.0", "Please use highlightElement now."), k(m);
  }
  Object.assign(e, {
    highlight: c,
    highlightAuto: _,
    highlightAll: B,
    highlightElement: k,
    // TODO: Remove with v12 API
    highlightBlock: O,
    configure: y,
    initHighlighting: C,
    initHighlightingOnLoad: D,
    registerLanguage: G,
    unregisterLanguage: M,
    listLanguages: q,
    getLanguage: x,
    registerAliases: te,
    autoDetection: ae,
    inherit: pn,
    addPlugin: se,
    removePlugin: E
  }), e.debugMode = function() {
    r = !1;
  }, e.safeMode = function() {
    r = !0;
  }, e.versionString = Al, e.regex = {
    concat: ze,
    lookahead: qn,
    either: zt,
    optional: Gi,
    anyNumberOfTimes: Fi
  };
  for (const m in tt)
    typeof tt[m] == "object" && Dn(tt[m]);
  return Object.assign(e, tt), e;
}, Ue = Xn({});
Ue.newInstance = () => Xn({});
var Nl = Ue;
Ue.HighlightJS = Ue;
Ue.default = Ue;
const oe = /* @__PURE__ */ Ui(Nl), gn = "[A-Za-z$_][0-9A-Za-z$_]*", Rl = [
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
], Ll = [
  "true",
  "false",
  "null",
  "undefined",
  "NaN",
  "Infinity"
], Yn = [
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
], Jn = [
  "Error",
  "EvalError",
  "InternalError",
  "RangeError",
  "ReferenceError",
  "SyntaxError",
  "TypeError",
  "URIError"
], Qn = [
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
], Bl = [
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
], zl = [].concat(
  Qn,
  Yn,
  Jn
);
function es(e) {
  const t = e.regex, n = (T, { after: v }) => {
    const U = "</" + T[0].slice(1);
    return T.input.indexOf(U, v) !== -1;
  }, s = gn, r = {
    begin: "<>",
    end: "</>"
  }, a = /<[A-Za-z0-9\\._:-]+\s*\/>/, l = {
    begin: /<[A-Za-z0-9\\._:-]+/,
    end: /\/[A-Za-z0-9\\._:-]+>|\/>/,
    /**
     * @param {RegExpMatchArray} match
     * @param {CallbackResponse} response
     */
    isTrulyOpeningTag: (T, v) => {
      const U = T[0].length + T.index, ne = T.input[U];
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
      ne === ">" && (n(T, { after: U }) || v.ignoreMatch());
      let ue;
      const _e = T.input.substring(U);
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
    $pattern: gn,
    keyword: Rl,
    literal: Ll,
    built_in: zl,
    "variable.language": Bl
  }, o = "[0-9](_?[0-9])*", d = `\\.(${o})`, c = "0|[1-9](_?[0-9])*|0[0-7]*[89][0-9]*", p = {
    className: "number",
    variants: [
      // DecimalLiteral
      { begin: `(\\b(${c})((${d})|\\.)?|(${d}))[eE][+-]?(${o})\\b` },
      { begin: `\\b(${c})\\b((${d})\\b|\\.)?|(${d})\\b` },
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
  }, u = {
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
        u
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
        u
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
        u
      ],
      subLanguage: "graphql"
    }
  }, y = {
    className: "string",
    begin: "`",
    end: "`",
    contains: [
      e.BACKSLASH_ESCAPE,
      u
    ]
  }, D = {
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
  }, L = [
    e.APOS_STRING_MODE,
    e.QUOTE_STRING_MODE,
    _,
    f,
    k,
    y,
    // Skip numbers when they are part of a variable name
    { match: /\$\d+/ },
    p
    // This is intentional:
    // See https://github.com/highlightjs/highlight.js/issues/3288
    // hljs.REGEXP_MODE
  ];
  u.contains = L.concat({
    // we need to pair up {} inside our subst to prevent
    // it from ending too early by matching another }
    begin: /\{/,
    end: /\}/,
    keywords: i,
    contains: [
      "self"
    ].concat(L)
  });
  const B = [].concat(D, u.contains), G = B.concat([
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
    contains: G
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
  }, x = {
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
        ...Yn,
        ...Jn
      ]
    }
  }, te = {
    label: "use_strict",
    className: "meta",
    relevance: 10,
    begin: /^\s*['"]use (strict|asm)['"]/
  }, ae = {
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
  function se(T) {
    return t.concat("(?!", T.join("|"), ")");
  }
  const E = {
    match: t.concat(
      /\b/,
      se([
        ...Qn,
        "super",
        "import"
      ].map((T) => `${T}\\s*\\(`)),
      s,
      t.lookahead(/\s*\(/)
    ),
    className: "title.function",
    relevance: 0
  }, I = {
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
  }, m = "(\\([^()]*(\\([^()]*(\\([^()]*\\)[^()]*)*\\)[^()]*)*\\)|" + e.UNDERSCORE_IDENT_RE + ")\\s*=>", w = {
    match: [
      /const|var|let/,
      /\s+/,
      s,
      /\s*/,
      /=\s*/,
      /(async\s*)?/,
      // async is optional
      t.lookahead(m)
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
    exports: { PARAMS_CONTAINS: G, CLASS_REFERENCE: x },
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
      y,
      D,
      // Skip numbers when they are part of a variable name
      { match: /\$\d+/ },
      p,
      x,
      {
        scope: "attr",
        match: s + t.lookahead(":"),
        relevance: 0
      },
      w,
      {
        // "value" container
        begin: "(" + e.RE_STARTERS_RE + "|\\b(case|return|throw)\\b)\\s*",
        keywords: "return throw case",
        relevance: 0,
        contains: [
          D,
          e.REGEXP_MODE,
          {
            className: "function",
            // we have to count the parens to make sure we actually have the
            // correct bounding ( ) before the =>.  There could be any number of
            // sub-expressions inside also surrounded by parens.
            begin: m,
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
                    contains: G
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
              { match: a },
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
      ae,
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
      I,
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
const ot = "[A-Za-z$_][0-9A-Za-z$_]*", ts = [
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
], ns = [
  "true",
  "false",
  "null",
  "undefined",
  "NaN",
  "Infinity"
], ss = [
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
], rs = [
  "Error",
  "EvalError",
  "InternalError",
  "RangeError",
  "ReferenceError",
  "SyntaxError",
  "TypeError",
  "URIError"
], as = [
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
], os = [
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
], is = [].concat(
  as,
  ss,
  rs
);
function Pl(e) {
  const t = e.regex, n = (T, { after: v }) => {
    const U = "</" + T[0].slice(1);
    return T.input.indexOf(U, v) !== -1;
  }, s = ot, r = {
    begin: "<>",
    end: "</>"
  }, a = /<[A-Za-z0-9\\._:-]+\s*\/>/, l = {
    begin: /<[A-Za-z0-9\\._:-]+/,
    end: /\/[A-Za-z0-9\\._:-]+>|\/>/,
    /**
     * @param {RegExpMatchArray} match
     * @param {CallbackResponse} response
     */
    isTrulyOpeningTag: (T, v) => {
      const U = T[0].length + T.index, ne = T.input[U];
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
      ne === ">" && (n(T, { after: U }) || v.ignoreMatch());
      let ue;
      const _e = T.input.substring(U);
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
    $pattern: ot,
    keyword: ts,
    literal: ns,
    built_in: is,
    "variable.language": os
  }, o = "[0-9](_?[0-9])*", d = `\\.(${o})`, c = "0|[1-9](_?[0-9])*|0[0-7]*[89][0-9]*", p = {
    className: "number",
    variants: [
      // DecimalLiteral
      { begin: `(\\b(${c})((${d})|\\.)?|(${d}))[eE][+-]?(${o})\\b` },
      { begin: `\\b(${c})\\b((${d})\\b|\\.)?|(${d})\\b` },
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
  }, u = {
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
        u
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
        u
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
        u
      ],
      subLanguage: "graphql"
    }
  }, y = {
    className: "string",
    begin: "`",
    end: "`",
    contains: [
      e.BACKSLASH_ESCAPE,
      u
    ]
  }, D = {
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
  }, L = [
    e.APOS_STRING_MODE,
    e.QUOTE_STRING_MODE,
    _,
    f,
    k,
    y,
    // Skip numbers when they are part of a variable name
    { match: /\$\d+/ },
    p
    // This is intentional:
    // See https://github.com/highlightjs/highlight.js/issues/3288
    // hljs.REGEXP_MODE
  ];
  u.contains = L.concat({
    // we need to pair up {} inside our subst to prevent
    // it from ending too early by matching another }
    begin: /\{/,
    end: /\}/,
    keywords: i,
    contains: [
      "self"
    ].concat(L)
  });
  const B = [].concat(D, u.contains), G = B.concat([
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
    contains: G
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
  }, x = {
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
        ...ss,
        ...rs
      ]
    }
  }, te = {
    label: "use_strict",
    className: "meta",
    relevance: 10,
    begin: /^\s*['"]use (strict|asm)['"]/
  }, ae = {
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
  function se(T) {
    return t.concat("(?!", T.join("|"), ")");
  }
  const E = {
    match: t.concat(
      /\b/,
      se([
        ...as,
        "super",
        "import"
      ].map((T) => `${T}\\s*\\(`)),
      s,
      t.lookahead(/\s*\(/)
    ),
    className: "title.function",
    relevance: 0
  }, I = {
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
  }, m = "(\\([^()]*(\\([^()]*(\\([^()]*\\)[^()]*)*\\)[^()]*)*\\)|" + e.UNDERSCORE_IDENT_RE + ")\\s*=>", w = {
    match: [
      /const|var|let/,
      /\s+/,
      s,
      /\s*/,
      /=\s*/,
      /(async\s*)?/,
      // async is optional
      t.lookahead(m)
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
    exports: { PARAMS_CONTAINS: G, CLASS_REFERENCE: x },
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
      y,
      D,
      // Skip numbers when they are part of a variable name
      { match: /\$\d+/ },
      p,
      x,
      {
        scope: "attr",
        match: s + t.lookahead(":"),
        relevance: 0
      },
      w,
      {
        // "value" container
        begin: "(" + e.RE_STARTERS_RE + "|\\b(case|return|throw)\\b)\\s*",
        keywords: "return throw case",
        relevance: 0,
        contains: [
          D,
          e.REGEXP_MODE,
          {
            className: "function",
            // we have to count the parens to make sure we actually have the
            // correct bounding ( ) before the =>.  There could be any number of
            // sub-expressions inside also surrounded by parens.
            begin: m,
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
                    contains: G
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
              { match: a },
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
      ae,
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
      I,
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
function ls(e) {
  const t = e.regex, n = Pl(e), s = ot, r = [
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
  ], a = {
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
  }, o = [
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
  ], d = {
    $pattern: ot,
    keyword: ts.concat(o),
    literal: ns,
    built_in: is.concat(r),
    "variable.language": os
  }, c = {
    className: "meta",
    begin: "@" + s
  }, p = (k, y, C) => {
    const D = k.contains.findIndex((L) => L.label === y);
    if (D === -1)
      throw new Error("can not find mode to replace");
    k.contains.splice(D, 1, C);
  };
  Object.assign(n.keywords, d), n.exports.PARAMS_CONTAINS.push(c);
  const u = n.contains.find((k) => k.scope === "attr"), _ = Object.assign(
    {},
    u,
    { match: t.concat(s, t.lookahead(/\s*\?:/)) }
  );
  n.exports.PARAMS_CONTAINS.push([
    n.exports.CLASS_REFERENCE,
    // class reference for highlighting the params types
    u,
    // highlight the params key
    _
    // Added for optional property assignment highlighting
  ]), n.contains = n.contains.concat([
    c,
    a,
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
function cs(e) {
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
  ], a = [
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
    keyword: r.concat(a),
    built_in: t,
    literal: s
  }, i = e.inherit(e.TITLE_MODE, { begin: "[a-zA-Z](\\.?\\w)*" }), o = {
    className: "number",
    variants: [
      { begin: "\\b(0b[01']+)" },
      { begin: "(-?)\\b([\\d']+(\\.[\\d']*)?|\\.[\\d']+)(u|U|l|L|ul|UL|f|F|b|B)" },
      { begin: "(-?)(\\b0[xX][a-fA-F0-9']+|(\\b[\\d']+(\\.[\\d']*)?|\\.[\\d']+)([eE][-+]?[\\d']+)?)" }
    ],
    relevance: 0
  }, d = {
    className: "string",
    begin: /"""("*)(?!")(.|\n)*?"""\1/,
    relevance: 1
  }, c = {
    className: "string",
    begin: '@"',
    end: '"',
    contains: [{ begin: '""' }]
  }, p = e.inherit(c, { illegal: /\n/ }), u = {
    className: "subst",
    begin: /\{/,
    end: /\}/,
    keywords: l
  }, _ = e.inherit(u, { illegal: /\n/ }), f = {
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
      u
    ]
  }, y = e.inherit(k, {
    illegal: /\n/,
    contains: [
      { begin: /\{\{/ },
      { begin: /\}\}/ },
      { begin: '""' },
      _
    ]
  });
  u.contains = [
    k,
    f,
    c,
    e.APOS_STRING_MODE,
    e.QUOTE_STRING_MODE,
    o,
    e.C_BLOCK_COMMENT_MODE
  ], _.contains = [
    y,
    f,
    p,
    e.APOS_STRING_MODE,
    e.QUOTE_STRING_MODE,
    o,
    e.inherit(e.C_BLOCK_COMMENT_MODE, { illegal: /\n/ })
  ];
  const C = { variants: [
    d,
    k,
    f,
    c,
    e.APOS_STRING_MODE,
    e.QUOTE_STRING_MODE
  ] }, D = {
    begin: "<",
    end: ">",
    contains: [
      { beginKeywords: "in out" },
      i
    ]
  }, L = e.IDENT_RE + "(<" + e.IDENT_RE + "(\\s*,\\s*" + e.IDENT_RE + ")*>)?(\\[\\])?", B = {
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
      C,
      o,
      {
        beginKeywords: "class interface",
        relevance: 0,
        end: /[{;=]/,
        illegal: /[^\s:,]/,
        contains: [
          { beginKeywords: "where class" },
          i,
          D,
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
          D,
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
        begin: "(" + L + "\\s+)+" + e.IDENT_RE + "\\s*(<[^=]+>\\s*)?\\(",
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
              D
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
              C,
              o,
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
function Ut(e) {
  const t = e.regex, n = t.concat(/[\p{L}_]/u, t.optional(/[\p{L}0-9_.-]*:/u), /[\p{L}0-9_.-]*/u), s = /[\p{L}0-9._:-]+/u, r = {
    className: "symbol",
    begin: /&[a-z]+;|&#[0-9]+;|&#x[a-f0-9]+;/
  }, a = {
    begin: /\s/,
    contains: [
      {
        className: "keyword",
        begin: /#?[a-z_][a-z1-9_-]+/,
        illegal: /\n/
      }
    ]
  }, l = e.inherit(a, {
    begin: /\(/,
    end: /\)/
  }), i = e.inherit(e.APOS_STRING_MODE, { className: "string" }), o = e.inherit(e.QUOTE_STRING_MODE, { className: "string" }), d = {
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
          a,
          o,
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
                  a,
                  l,
                  o,
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
              o
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
        contains: [d],
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
        contains: [d],
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
            starts: d
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
const Dl = (e) => ({
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
}), Ul = [
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
], ql = [
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
], jl = [
  ...Ul,
  ...ql
], Hl = [
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
].sort().reverse(), Vl = [
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
].sort().reverse(), Fl = [
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
].sort().reverse(), Gl = [
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
function Wl(e) {
  const t = e.regex, n = Dl(e), s = { begin: /-(webkit|moz|ms|o)-(?=[a-z])/ }, r = "and or not only", a = /@-?\w[\w]*(-\w+)*/, l = "[a-zA-Z-][a-zA-Z0-9_-]*", i = [
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
          { begin: ":(" + Vl.join("|") + ")" },
          { begin: ":(:)?(" + Fl.join("|") + ")" }
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
        begin: "\\b(" + Gl.join("|") + ")\\b"
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
            begin: a
          },
          {
            begin: /\s/,
            endsWithParent: !0,
            excludeEnd: !0,
            relevance: 0,
            keywords: {
              $pattern: /[a-z-]+/,
              keyword: r,
              attribute: Hl.join(" ")
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
        begin: "\\b(" + jl.join("|") + ")\\b"
      }
    ]
  };
}
function Kl(e) {
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
function ds(e) {
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
  }, a = e.inherit(
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
  const o = {
    match: /\\"/
  }, d = {
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
  }, u = [
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
    binary: `(${u.join("|")})`,
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
  ], y = [
    "true",
    "false"
  ], C = { match: /(\/[a-z._-]+)+/ }, D = [
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
  ], L = [
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
  ], G = [
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
      literal: y,
      built_in: [
        ...D,
        ...L,
        // Shell modifiers
        "set",
        "shopt",
        ...B,
        ...G
      ]
    },
    contains: [
      _,
      // to catch known shells and boost relevancy
      e.SHEBANG(),
      // to catch unknown shells but still highlight the shebang
      f,
      p,
      a,
      l,
      C,
      i,
      o,
      d,
      c,
      n
    ]
  };
}
oe.registerLanguage("javascript", es);
oe.registerLanguage("js", es);
oe.registerLanguage("typescript", ls);
oe.registerLanguage("ts", ls);
oe.registerLanguage("csharp", cs);
oe.registerLanguage("cs", cs);
oe.registerLanguage("xml", Ut);
oe.registerLanguage("html", Ut);
oe.registerLanguage("vue", Ut);
oe.registerLanguage("css", Wl);
oe.registerLanguage("json", Kl);
oe.registerLanguage("bash", ds);
oe.registerLanguage("sh", ds);
const Zl = [
  "javascript",
  "typescript",
  "csharp",
  "xml",
  "css",
  "json",
  "bash"
], Xl = ["innerHTML"], Yl = ["innerHTML"], Jl = ["value"], Ql = /* @__PURE__ */ z({
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
    const n = e, s = t, r = re(!1), a = Y(() => {
      if (!n.lang || n.lang === "auto")
        return oe.highlightAuto(n.code, Zl).value;
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
    ), o = re(!1);
    async function d() {
      try {
        await navigator.clipboard.writeText(n.code), o.value = !0, setTimeout(() => {
          o.value = !1;
        }, 1200);
      } catch {
      }
    }
    function c(p) {
      s("update:code", p.target.value);
    }
    return (p, u) => (g(), b("div", {
      class: h(p.$style.block)
    }, [
      i.value ? (g(), b("div", {
        key: 0,
        class: h(p.$style.head)
      }, [
        e.filename ? (g(), b("span", {
          key: 0,
          class: h(p.$style.filename)
        }, S(e.filename), 3)) : N("", !0),
        e.lang && e.lang !== "auto" ? (g(), b("span", {
          key: 1,
          class: h(p.$style.lang)
        }, S(e.lang), 3)) : N("", !0),
        $("div", {
          class: h(p.$style.spacer)
        }, null, 2),
        e.readonly ? N("", !0) : (g(), b("button", {
          key: 2,
          type: "button",
          class: h([p.$style.btn, r.value && p.$style.btnOn]),
          onClick: u[0] || (u[0] = (_) => r.value = !r.value)
        }, S(r.value ? "done" : "edit"), 3)),
        e.copy ? (g(), b("button", {
          key: 3,
          type: "button",
          class: h(p.$style.btn),
          onClick: d
        }, S(o.value ? "copied" : "copy"), 3)) : N("", !0)
      ], 2)) : N("", !0),
      r.value ? (g(), b("div", {
        key: 2,
        class: h(p.$style.editorWrap)
      }, [
        $("pre", {
          class: h(p.$style.editorView),
          "aria-hidden": "true"
        }, [
          $("code", {
            class: "hljs",
            innerHTML: a.value
          }, null, 8, Yl),
          u[1] || (u[1] = fe())
        ], 2),
        $("textarea", {
          class: h(p.$style.editorInput),
          value: e.code,
          spellcheck: "false",
          autocorrect: "off",
          autocapitalize: "off",
          onInput: c
        }, null, 42, Jl)
      ], 2)) : (g(), b("pre", {
        key: 1,
        class: h(p.$style.pre)
      }, [
        $("code", {
          class: h(["hljs", p.$style.code]),
          innerHTML: a.value
        }, null, 10, Xl)
      ], 2))
    ], 2));
  }
}), ec = "_block_aaqrg_2", tc = "_head_aaqrg_9", nc = "_filename_aaqrg_18", sc = "_lang_aaqrg_24", rc = "_spacer_aaqrg_35", ac = "_btn_aaqrg_37", oc = "_btnOn_aaqrg_52", ic = "_pre_aaqrg_58", lc = "_code_aaqrg_65", cc = "_editorWrap_aaqrg_80", dc = "_editorView_aaqrg_86", uc = "_editorInput_aaqrg_86", hc = {
  block: ec,
  head: tc,
  filename: nc,
  lang: sc,
  spacer: rc,
  btn: ac,
  btnOn: oc,
  pre: ic,
  code: lc,
  editorWrap: cc,
  editorView: dc,
  editorInput: uc
}, pc = {
  $style: hc
}, pm = /* @__PURE__ */ P(Ql, [["__cssModules", pc]]), fc = ["innerHTML"], gc = ["innerHTML"], yc = ["value"], bc = /* @__PURE__ */ z({
  __name: "TextVarBlock",
  props: {
    text: {},
    filename: {},
    copy: { type: Boolean, default: !0 },
    readonly: { type: Boolean, default: !0 }
  },
  emits: ["update:text"],
  setup(e, { emit: t }) {
    const n = e, s = t, r = re(!1), a = /(\{\{\s*\$[a-zA-Z_][a-zA-Z0-9_]*\s*\}\})/g;
    function l(u) {
      return u.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    }
    const i = Y(() => n.text.split(a).map((_, f) => f % 2 === 1 ? `<span class="tvb-var">${l(_)}</span>` : l(_)).join("")), o = Y(
      () => !!n.filename || n.copy || !n.readonly
    ), d = re(!1);
    async function c() {
      try {
        await navigator.clipboard.writeText(n.text), d.value = !0, setTimeout(() => {
          d.value = !1;
        }, 1200);
      } catch {
      }
    }
    function p(u) {
      s("update:text", u.target.value);
    }
    return (u, _) => (g(), b("div", {
      class: h(u.$style.block)
    }, [
      o.value ? (g(), b("div", {
        key: 0,
        class: h(u.$style.head)
      }, [
        e.filename ? (g(), b("span", {
          key: 0,
          class: h(u.$style.filename)
        }, S(e.filename), 3)) : N("", !0),
        $("div", {
          class: h(u.$style.spacer)
        }, null, 2),
        e.readonly ? N("", !0) : (g(), b("button", {
          key: 1,
          type: "button",
          class: h([u.$style.btn, r.value && u.$style.btnOn]),
          onClick: _[0] || (_[0] = (f) => r.value = !r.value)
        }, S(r.value ? "done" : "edit"), 3)),
        e.copy ? (g(), b("button", {
          key: 2,
          type: "button",
          class: h(u.$style.btn),
          onClick: c
        }, S(d.value ? "copied" : "copy"), 3)) : N("", !0)
      ], 2)) : N("", !0),
      r.value ? (g(), b("div", {
        key: 2,
        class: h(u.$style.editorWrap)
      }, [
        $("pre", {
          class: h(u.$style.editorView),
          "aria-hidden": "true"
        }, [
          $("span", { innerHTML: i.value }, null, 8, gc),
          _[1] || (_[1] = fe())
        ], 2),
        $("textarea", {
          class: h(u.$style.editorInput),
          value: e.text,
          spellcheck: "false",
          autocorrect: "off",
          autocapitalize: "off",
          onInput: p
        }, null, 42, yc)
      ], 2)) : (g(), b("pre", {
        key: 1,
        class: h(u.$style.pre)
      }, [
        $("span", {
          class: h(u.$style.text),
          innerHTML: i.value
        }, null, 10, fc)
      ], 2))
    ], 2));
  }
}), mc = "_block_1pww8_2", kc = "_head_1pww8_9", _c = "_filename_1pww8_18", wc = "_spacer_1pww8_24", $c = "_btn_1pww8_26", vc = "_btnOn_1pww8_41", Ec = "_pre_1pww8_47", Sc = "_text_1pww8_56", Tc = "_editorView_1pww8_67", xc = "_editorWrap_1pww8_78", Mc = "_editorInput_1pww8_84", Cc = {
  block: mc,
  head: kc,
  filename: _c,
  spacer: wc,
  btn: $c,
  btnOn: vc,
  pre: Ec,
  text: Sc,
  editorView: Tc,
  editorWrap: xc,
  editorInput: Mc
}, Ac = {
  $style: Cc
}, fm = /* @__PURE__ */ P(bc, [["__cssModules", Ac]]), Ic = { class: "seg-wrap" }, Oc = {
  class: "seg",
  role: "tablist"
}, Nc = ["aria-selected", "onClick"], Rc = {
  key: 0,
  class: "seg-hint"
}, Lc = /* @__PURE__ */ z({
  __name: "Segmented",
  props: {
    modelValue: {},
    options: {}
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const n = e, s = t, r = () => {
      var a;
      return (a = n.options.find((l) => l.value === n.modelValue)) == null ? void 0 : a.hint;
    };
    return (a, l) => (g(), b("div", Ic, [
      $("div", Oc, [
        (g(!0), b(le, null, ye(e.options, (i) => (g(), b("button", {
          key: i.value,
          type: "button",
          role: "tab",
          "aria-selected": i.value === e.modelValue,
          class: h(["seg-btn", { active: i.value === e.modelValue }]),
          onClick: (o) => s("update:modelValue", i.value)
        }, S(i.label), 11, Nc))), 128))
      ]),
      r() ? (g(), b("p", Rc, S(r()), 1)) : N("", !0)
    ]));
  }
}), gm = /* @__PURE__ */ P(Lc, [["__scopeId", "data-v-15af1c50"]]), Bc = { class: "varchips" }, zc = { class: "vc-label" }, Pc = ["title", "onClick"], Dc = { class: "vc-tok" }, Uc = /* @__PURE__ */ z({
  __name: "VarChips",
  props: {
    vars: {},
    label: {}
  },
  emits: ["insert"],
  setup(e, { emit: t }) {
    const n = t;
    return (s, r) => (g(), b("div", Bc, [
      $("span", zc, S(e.label ?? "Insert:"), 1),
      (g(!0), b(le, null, ye(e.vars, (a) => (g(), b("button", {
        key: a.token,
        type: "button",
        class: "vc-chip",
        title: a.desc ?? a.token,
        onClick: (l) => n("insert", a.token)
      }, [
        fe(S(a.label) + " ", 1),
        $("code", Dc, S(a.token), 1)
      ], 8, Pc))), 128))
    ]));
  }
}), ym = /* @__PURE__ */ P(Uc, [["__scopeId", "data-v-f3af8416"]]);
function qt() {
  return { async: !1, breaks: !1, extensions: null, gfm: !0, hooks: null, pedantic: !1, renderer: null, silent: !1, tokenizer: null, walkTokens: null };
}
var Pe = qt();
function us(e) {
  Pe = e;
}
var Ne = { exec: () => null };
function W(e, t = "") {
  let n = typeof e == "string" ? e : e.source, s = { replace: (r, a) => {
    let l = typeof a == "string" ? a : a.source;
    return l = l.replace(pe.caret, "$1"), n = n.replace(r, l), s;
  }, getRegex: () => new RegExp(n, t) };
  return s;
}
var qc = ((e = "") => {
  try {
    return !!new RegExp("(?<=1)(?<!1)" + e);
  } catch {
    return !1;
  }
})(), pe = { codeRemoveIndent: /^(?: {1,4}| {0,3}\t)/gm, outputLinkReplace: /\\([\[\]])/g, indentCodeCompensation: /^(\s+)(?:```)/, beginningSpace: /^\s+/, endingHash: /#$/, startingSpaceChar: /^ /, endingSpaceChar: / $/, nonSpaceChar: /[^ ]/, newLineCharGlobal: /\n/g, tabCharGlobal: /\t/g, multipleSpaceGlobal: /\s+/g, blankLine: /^[ \t]*$/, doubleBlankLine: /\n[ \t]*\n[ \t]*$/, blockquoteStart: /^ {0,3}>/, blockquoteSetextReplace: /\n {0,3}((?:=+|-+) *)(?=\n|$)/g, blockquoteSetextReplace2: /^ {0,3}>[ \t]?/gm, listReplaceNesting: /^ {1,4}(?=( {4})*[^ ])/g, listIsTask: /^\[[ xX]\] +\S/, listReplaceTask: /^\[[ xX]\] +/, listTaskCheckbox: /\[[ xX]\]/, anyLine: /\n.*\n/, hrefBrackets: /^<(.*)>$/, tableDelimiter: /[:|]/, tableAlignChars: /^\||\| *$/g, tableRowBlankLine: /\n[ \t]*$/, tableAlignRight: /^ *-+: *$/, tableAlignCenter: /^ *:-+: *$/, tableAlignLeft: /^ *:-+ *$/, startATag: /^<a /i, endATag: /^<\/a>/i, startPreScriptTag: /^<(pre|code|kbd|script)(\s|>)/i, endPreScriptTag: /^<\/(pre|code|kbd|script)(\s|>)/i, startAngleBracket: /^</, endAngleBracket: />$/, pedanticHrefTitle: /^([^'"]*[^\s])\s+(['"])(.*)\2/, unicodeAlphaNumeric: /[\p{L}\p{N}]/u, escapeTest: /[&<>"']/, escapeReplace: /[&<>"']/g, escapeTestNoEncode: /[<>"']|&(?!(#\d{1,7}|#[Xx][a-fA-F0-9]{1,6}|\w+);)/, escapeReplaceNoEncode: /[<>"']|&(?!(#\d{1,7}|#[Xx][a-fA-F0-9]{1,6}|\w+);)/g, caret: /(^|[^\[])\^/g, percentDecode: /%25/g, findPipe: /\|/g, splitPipe: / \|/, slashPipe: /\\\|/g, carriageReturn: /\r\n|\r/g, spaceLine: /^ +$/gm, notSpaceStart: /^\S*/, endingNewline: /\n$/, listItemRegex: (e) => new RegExp(`^( {0,3}${e})((?:[	 ][^\\n]*)?(?:\\n|$))`), nextBulletRegex: (e) => new RegExp(`^ {0,${Math.min(3, e - 1)}}(?:[*+-]|\\d{1,9}[.)])((?:[ 	][^\\n]*)?(?:\\n|$))`), hrRegex: (e) => new RegExp(`^ {0,${Math.min(3, e - 1)}}((?:- *){3,}|(?:_ *){3,}|(?:\\* *){3,})(?:\\n+|$)`), fencesBeginRegex: (e) => new RegExp(`^ {0,${Math.min(3, e - 1)}}(?:\`\`\`|~~~)`), headingBeginRegex: (e) => new RegExp(`^ {0,${Math.min(3, e - 1)}}#`), htmlBeginRegex: (e) => new RegExp(`^ {0,${Math.min(3, e - 1)}}<(?:[a-z].*>|!--)`, "i"), blockquoteBeginRegex: (e) => new RegExp(`^ {0,${Math.min(3, e - 1)}}>`) }, jc = /^(?:[ \t]*(?:\n|$))+/, Hc = /^((?: {4}| {0,3}\t)[^\n]+(?:\n(?:[ \t]*(?:\n|$))*)?)+/, Vc = /^ {0,3}(`{3,}(?=[^`\n]*(?:\n|$))|~{3,})([^\n]*)(?:\n|$)(?:|([\s\S]*?)(?:\n|$))(?: {0,3}\1[~`]* *(?=\n|$)|$)/, Je = /^ {0,3}((?:-[\t ]*){3,}|(?:_[ \t]*){3,}|(?:\*[ \t]*){3,})(?:\n+|$)/, Fc = /^ {0,3}(#{1,6})(?=\s|$)(.*)(?:\n+|$)/, jt = / {0,3}(?:[*+-]|\d{1,9}[.)])/, hs = /^(?!bull |blockCode|fences|blockquote|heading|html|table)((?:.|\n(?!\s*?\n|bull |blockCode|fences|blockquote|heading|html|table))+?)\n {0,3}(=+|-+) *(?:\n+|$)/, ps = W(hs).replace(/bull/g, jt).replace(/blockCode/g, /(?: {4}| {0,3}\t)/).replace(/fences/g, / {0,3}(?:`{3,}|~{3,})/).replace(/blockquote/g, / {0,3}>/).replace(/heading/g, / {0,3}#{1,6}/).replace(/html/g, / {0,3}<[^\n>]+>\n/).replace(/\|table/g, "").getRegex(), Gc = W(hs).replace(/bull/g, jt).replace(/blockCode/g, /(?: {4}| {0,3}\t)/).replace(/fences/g, / {0,3}(?:`{3,}|~{3,})/).replace(/blockquote/g, / {0,3}>/).replace(/heading/g, / {0,3}#{1,6}/).replace(/html/g, / {0,3}<[^\n>]+>\n/).replace(/table/g, / {0,3}\|?(?:[:\- ]*\|)+[\:\- ]*\n/).getRegex(), Ht = /^([^\n]+(?:\n(?!hr|heading|lheading|blockquote|fences|list|html|table| +\n)[^\n]+)*)/, Wc = /^[^\n]+/, Vt = /(?!\s*\])(?:\\[\s\S]|[^\[\]\\])+/, Kc = W(/^ {0,3}\[(label)\]: *(?:\n[ \t]*)?([^<\s][^\s]*|<.*?>)(?:(?: +(?:\n[ \t]*)?| *\n[ \t]*)(title))? *(?:\n+|$)/).replace("label", Vt).replace("title", /(?:"(?:\\"?|[^"\\])*"|'[^'\n]*(?:\n[^'\n]+)*\n?'|\([^()]*\))/).getRegex(), Zc = W(/^(bull)([ \t][^\n]+?)?(?:\n|$)/).replace(/bull/g, jt).getRegex(), pt = "address|article|aside|base|basefont|blockquote|body|caption|center|col|colgroup|dd|details|dialog|dir|div|dl|dt|fieldset|figcaption|figure|footer|form|frame|frameset|h[1-6]|head|header|hr|html|iframe|legend|li|link|main|menu|menuitem|meta|nav|noframes|ol|optgroup|option|p|param|search|section|summary|table|tbody|td|tfoot|th|thead|title|tr|track|ul", Ft = /<!--(?:-?>|[\s\S]*?(?:-->|$))/, Xc = W("^ {0,3}(?:<(script|pre|style|textarea)[\\s>][\\s\\S]*?(?:</\\1>[^\\n]*\\n+|$)|comment[^\\n]*(\\n+|$)|<\\?[\\s\\S]*?(?:\\?>\\n*|$)|<![A-Z][\\s\\S]*?(?:>\\n*|$)|<!\\[CDATA\\[[\\s\\S]*?(?:\\]\\]>\\n*|$)|</?(tag)(?: +|\\n|/?>)[\\s\\S]*?(?:(?:\\n[ 	]*)+\\n|$)|<(?!script|pre|style|textarea)([a-z][\\w-]*)(?:attribute)*? */?>(?=[ \\t]*(?:\\n|$))[\\s\\S]*?(?:(?:\\n[ 	]*)+\\n|$)|</(?!script|pre|style|textarea)[a-z][\\w-]*\\s*>(?=[ \\t]*(?:\\n|$))[\\s\\S]*?(?:(?:\\n[ 	]*)+\\n|$))", "i").replace("comment", Ft).replace("tag", pt).replace("attribute", / +[a-zA-Z:_][\w.:-]*(?: *= *"[^"\n]*"| *= *'[^'\n]*'| *= *[^\s"'=<>`]+)?/).getRegex(), fs = W(Ht).replace("hr", Je).replace("heading", " {0,3}#{1,6}(?:\\s|$)").replace("|lheading", "").replace("|table", "").replace("blockquote", " {0,3}>").replace("fences", " {0,3}(?:`{3,}(?=[^`\\n]*\\n)|~{3,})[^\\n]*\\n").replace("list", " {0,3}(?:[*+-]|1[.)])[ \\t]").replace("html", "</?(?:tag)(?: +|\\n|/?>)|<(?:script|pre|style|textarea|!--)").replace("tag", pt).getRegex(), Yc = W(/^( {0,3}> ?(paragraph|[^\n]*)(?:\n|$))+/).replace("paragraph", fs).getRegex(), Gt = { blockquote: Yc, code: Hc, def: Kc, fences: Vc, heading: Fc, hr: Je, html: Xc, lheading: ps, list: Zc, newline: jc, paragraph: fs, table: Ne, text: Wc }, yn = W("^ *([^\\n ].*)\\n {0,3}((?:\\| *)?:?-+:? *(?:\\| *:?-+:? *)*(?:\\| *)?)(?:\\n((?:(?! *\\n|hr|heading|blockquote|code|fences|list|html).*(?:\\n|$))*)\\n*|$)").replace("hr", Je).replace("heading", " {0,3}#{1,6}(?:\\s|$)").replace("blockquote", " {0,3}>").replace("code", "(?: {4}| {0,3}	)[^\\n]").replace("fences", " {0,3}(?:`{3,}(?=[^`\\n]*\\n)|~{3,})[^\\n]*\\n").replace("list", " {0,3}(?:[*+-]|1[.)])[ \\t]").replace("html", "</?(?:tag)(?: +|\\n|/?>)|<(?:script|pre|style|textarea|!--)").replace("tag", pt).getRegex(), Jc = { ...Gt, lheading: Gc, table: yn, paragraph: W(Ht).replace("hr", Je).replace("heading", " {0,3}#{1,6}(?:\\s|$)").replace("|lheading", "").replace("table", yn).replace("blockquote", " {0,3}>").replace("fences", " {0,3}(?:`{3,}(?=[^`\\n]*\\n)|~{3,})[^\\n]*\\n").replace("list", " {0,3}(?:[*+-]|1[.)])[ \\t]").replace("html", "</?(?:tag)(?: +|\\n|/?>)|<(?:script|pre|style|textarea|!--)").replace("tag", pt).getRegex() }, Qc = { ...Gt, html: W(`^ *(?:comment *(?:\\n|\\s*$)|<(tag)[\\s\\S]+?</\\1> *(?:\\n{2,}|\\s*$)|<tag(?:"[^"]*"|'[^']*'|\\s[^'"/>\\s]*)*?/?> *(?:\\n{2,}|\\s*$))`).replace("comment", Ft).replace(/tag/g, "(?!(?:a|em|strong|small|s|cite|q|dfn|abbr|data|time|code|var|samp|kbd|sub|sup|i|b|u|mark|ruby|rt|rp|bdi|bdo|span|br|wbr|ins|del|img)\\b)\\w+(?!:|[^\\w\\s@]*@)\\b").getRegex(), def: /^ *\[([^\]]+)\]: *<?([^\s>]+)>?(?: +(["(][^\n]+[")]))? *(?:\n+|$)/, heading: /^(#{1,6})(.*)(?:\n+|$)/, fences: Ne, lheading: /^(.+?)\n {0,3}(=+|-+) *(?:\n+|$)/, paragraph: W(Ht).replace("hr", Je).replace("heading", ` *#{1,6} *[^
]`).replace("lheading", ps).replace("|table", "").replace("blockquote", " {0,3}>").replace("|fences", "").replace("|list", "").replace("|html", "").replace("|tag", "").getRegex() }, ed = /^\\([!"#$%&'()*+,\-./:;<=>?@\[\]\\^_`{|}~])/, td = /^(`+)([^`]|[^`][\s\S]*?[^`])\1(?!`)/, gs = /^( {2,}|\\)\n(?!\s*$)/, nd = /^(`+|[^`])(?:(?= {2,}\n)|[\s\S]*?(?:(?=[\\<!\[`*_]|\b_|$)|[^ ](?= {2,}\n)))/, qe = /[\p{P}\p{S}]/u, ft = /[\s\p{P}\p{S}]/u, Wt = /[^\s\p{P}\p{S}]/u, sd = W(/^((?![*_])punctSpace)/, "u").replace(/punctSpace/g, ft).getRegex(), ys = /(?!~)[\p{P}\p{S}]/u, rd = /(?!~)[\s\p{P}\p{S}]/u, ad = /(?:[^\s\p{P}\p{S}]|~)/u, od = W(/link|precode-code|html/, "g").replace("link", /\[(?:[^\[\]`]|(?<a>`+)[^`]+\k<a>(?!`))*?\]\((?:\\[\s\S]|[^\\\(\)]|\((?:\\[\s\S]|[^\\\(\)])*\))*\)/).replace("precode-", qc ? "(?<!`)()" : "(^^|[^`])").replace("code", /(?<b>`+)[^`]+\k<b>(?!`)/).replace("html", /<(?! )[^<>]*?>/).getRegex(), bs = /^(?:\*+(?:((?!\*)punct)|([^\s*]))?)|^_+(?:((?!_)punct)|([^\s_]))?/, id = W(bs, "u").replace(/punct/g, qe).getRegex(), ld = W(bs, "u").replace(/punct/g, ys).getRegex(), ms = "^[^_*]*?__[^_*]*?\\*[^_*]*?(?=__)|[^*]+(?=[^*])|(?!\\*)punct(\\*+)(?=[\\s]|$)|notPunctSpace(\\*+)(?!\\*)(?=punctSpace|$)|(?!\\*)punctSpace(\\*+)(?=notPunctSpace)|[\\s](\\*+)(?!\\*)(?=punct)|(?!\\*)punct(\\*+)(?!\\*)(?=punct)|notPunctSpace(\\*+)(?=notPunctSpace)", cd = W(ms, "gu").replace(/notPunctSpace/g, Wt).replace(/punctSpace/g, ft).replace(/punct/g, qe).getRegex(), dd = W(ms, "gu").replace(/notPunctSpace/g, ad).replace(/punctSpace/g, rd).replace(/punct/g, ys).getRegex(), ud = W("^[^_*]*?\\*\\*[^_*]*?_[^_*]*?(?=\\*\\*)|[^_]+(?=[^_])|(?!_)punct(_+)(?=[\\s]|$)|notPunctSpace(_+)(?!_)(?=punctSpace|$)|(?!_)punctSpace(_+)(?=notPunctSpace)|[\\s](_+)(?!_)(?=punct)|(?!_)punct(_+)(?!_)(?=punct)", "gu").replace(/notPunctSpace/g, Wt).replace(/punctSpace/g, ft).replace(/punct/g, qe).getRegex(), hd = W(/^~~?(?:((?!~)punct)|[^\s~])/, "u").replace(/punct/g, qe).getRegex(), pd = "^[^~]+(?=[^~])|(?!~)punct(~~?)(?=[\\s]|$)|notPunctSpace(~~?)(?!~)(?=punctSpace|$)|(?!~)punctSpace(~~?)(?=notPunctSpace)|[\\s](~~?)(?!~)(?=punct)|(?!~)punct(~~?)(?!~)(?=punct)|notPunctSpace(~~?)(?=notPunctSpace)", fd = W(pd, "gu").replace(/notPunctSpace/g, Wt).replace(/punctSpace/g, ft).replace(/punct/g, qe).getRegex(), gd = W(/\\(punct)/, "gu").replace(/punct/g, qe).getRegex(), yd = W(/^<(scheme:[^\s\x00-\x1f<>]*|email)>/).replace("scheme", /[a-zA-Z][a-zA-Z0-9+.-]{1,31}/).replace("email", /[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+(@)[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+(?![-_])/).getRegex(), bd = W(Ft).replace("(?:-->|$)", "-->").getRegex(), md = W("^comment|^</[a-zA-Z][\\w:-]*\\s*>|^<[a-zA-Z][\\w-]*(?:attribute)*?\\s*/?>|^<\\?[\\s\\S]*?\\?>|^<![a-zA-Z]+\\s[\\s\\S]*?>|^<!\\[CDATA\\[[\\s\\S]*?\\]\\]>").replace("comment", bd).replace("attribute", /\s+[a-zA-Z:_][\w.:-]*(?:\s*=\s*"[^"]*"|\s*=\s*'[^']*'|\s*=\s*[^\s"'=<>`]+)?/).getRegex(), it = /(?:\[(?:\\[\s\S]|[^\[\]\\])*\]|\\[\s\S]|`+(?!`)[^`]*?`+(?!`)|``+(?=\])|[^\[\]\\`])*?/, kd = W(/^!?\[(label)\]\(\s*(href)(?:(?:[ \t]+(?:\n[ \t]*)?|\n[ \t]*)(title))?\s*\)/).replace("label", it).replace("href", /<(?:\\.|[^\n<>\\])+>|[^ \t\n\x00-\x1f]*/).replace("title", /"(?:\\"?|[^"\\])*"|'(?:\\'?|[^'\\])*'|\((?:\\\)?|[^)\\])*\)/).getRegex(), ks = W(/^!?\[(label)\]\[(ref)\]/).replace("label", it).replace("ref", Vt).getRegex(), _s = W(/^!?\[(ref)\](?:\[\])?/).replace("ref", Vt).getRegex(), _d = W("reflink|nolink(?!\\()", "g").replace("reflink", ks).replace("nolink", _s).getRegex(), bn = /[hH][tT][tT][pP][sS]?|[fF][tT][pP]/, Kt = { _backpedal: Ne, anyPunctuation: gd, autolink: yd, blockSkip: od, br: gs, code: td, del: Ne, delLDelim: Ne, delRDelim: Ne, emStrongLDelim: id, emStrongRDelimAst: cd, emStrongRDelimUnd: ud, escape: ed, link: kd, nolink: _s, punctuation: sd, reflink: ks, reflinkSearch: _d, tag: md, text: nd, url: Ne }, wd = { ...Kt, link: W(/^!?\[(label)\]\((.*?)\)/).replace("label", it).getRegex(), reflink: W(/^!?\[(label)\]\s*\[([^\]]*)\]/).replace("label", it).getRegex() }, At = { ...Kt, emStrongRDelimAst: dd, emStrongLDelim: ld, delLDelim: hd, delRDelim: fd, url: W(/^((?:protocol):\/\/|www\.)(?:[a-zA-Z0-9\-]+\.?)+[^\s<]*|^email/).replace("protocol", bn).replace("email", /[A-Za-z0-9._+-]+(@)[a-zA-Z0-9-_]+(?:\.[a-zA-Z0-9-_]*[a-zA-Z0-9])+(?![-_])/).getRegex(), _backpedal: /(?:[^?!.,:;*_'"~()&]+|\([^)]*\)|&(?![a-zA-Z0-9]+;$)|[?!.,:;*_'"~)]+(?!$))+/, del: /^(~~?)(?=[^\s~])((?:\\[\s\S]|[^\\])*?(?:\\[\s\S]|[^\s~\\]))\1(?=[^~]|$)/, text: W(/^([`~]+|[^`~])(?:(?= {2,}\n)|(?=[a-zA-Z0-9.!#$%&'*+\/=?_`{\|}~-]+@)|[\s\S]*?(?:(?=[\\<!\[`*~_]|\b_|protocol:\/\/|www\.|$)|[^ ](?= {2,}\n)|[^a-zA-Z0-9.!#$%&'*+\/=?_`{\|}~-](?=[a-zA-Z0-9.!#$%&'*+\/=?_`{\|}~-]+@)))/).replace("protocol", bn).getRegex() }, $d = { ...At, br: W(gs).replace("{2,}", "*").getRegex(), text: W(At.text).replace("\\b_", "\\b_| {2,}\\n").replace(/\{2,\}/g, "*").getRegex() }, nt = { normal: Gt, gfm: Jc, pedantic: Qc }, je = { normal: Kt, gfm: At, breaks: $d, pedantic: wd }, vd = { "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }, mn = (e) => vd[e];
function Ee(e, t) {
  if (t) {
    if (pe.escapeTest.test(e)) return e.replace(pe.escapeReplace, mn);
  } else if (pe.escapeTestNoEncode.test(e)) return e.replace(pe.escapeReplaceNoEncode, mn);
  return e;
}
function kn(e) {
  try {
    e = encodeURI(e).replace(pe.percentDecode, "%");
  } catch {
    return null;
  }
  return e;
}
function _n(e, t) {
  var a;
  let n = e.replace(pe.findPipe, (l, i, o) => {
    let d = !1, c = i;
    for (; --c >= 0 && o[c] === "\\"; ) d = !d;
    return d ? "|" : " |";
  }), s = n.split(pe.splitPipe), r = 0;
  if (s[0].trim() || s.shift(), s.length > 0 && !((a = s.at(-1)) != null && a.trim()) && s.pop(), t) if (s.length > t) s.splice(t);
  else for (; s.length < t; ) s.push("");
  for (; r < s.length; r++) s[r] = s[r].trim().replace(pe.slashPipe, "|");
  return s;
}
function Ce(e, t, n) {
  let s = e.length;
  if (s === 0) return "";
  let r = 0;
  for (; r < s && e.charAt(s - r - 1) === t; )
    r++;
  return e.slice(0, s - r);
}
function wn(e) {
  let t = e.split(`
`), n = t.length - 1;
  for (; n >= 0 && pe.blankLine.test(t[n]); ) n--;
  return t.length - n <= 2 ? e : t.slice(0, n + 1).join(`
`);
}
function Ed(e, t) {
  if (e.indexOf(t[1]) === -1) return -1;
  let n = 0;
  for (let s = 0; s < e.length; s++) if (e[s] === "\\") s++;
  else if (e[s] === t[0]) n++;
  else if (e[s] === t[1] && (n--, n < 0)) return s;
  return n > 0 ? -2 : -1;
}
function Sd(e, t = 0) {
  let n = t, s = "";
  for (let r of e) if (r === "	") {
    let a = 4 - n % 4;
    s += " ".repeat(a), n += a;
  } else s += r, n++;
  return s;
}
function $n(e, t, n, s, r) {
  let a = t.href, l = t.title || null, i = e[1].replace(r.other.outputLinkReplace, "$1");
  s.state.inLink = !0;
  let o = { type: e[0].charAt(0) === "!" ? "image" : "link", raw: n, href: a, title: l, text: i, tokens: s.inlineTokens(i) };
  return s.state.inLink = !1, o;
}
function Td(e, t, n) {
  let s = e.match(n.other.indentCodeCompensation);
  if (s === null) return t;
  let r = s[1];
  return t.split(`
`).map((a) => {
    let l = a.match(n.other.beginningSpace);
    if (l === null) return a;
    let [i] = l;
    return i.length >= r.length ? a.slice(r.length) : a;
  }).join(`
`);
}
var lt = class {
  constructor(e) {
    V(this, "options");
    V(this, "rules");
    V(this, "lexer");
    this.options = e || Pe;
  }
  space(e) {
    let t = this.rules.block.newline.exec(e);
    if (t && t[0].length > 0) return { type: "space", raw: t[0] };
  }
  code(e) {
    let t = this.rules.block.code.exec(e);
    if (t) {
      let n = this.options.pedantic ? t[0] : wn(t[0]), s = n.replace(this.rules.other.codeRemoveIndent, "");
      return { type: "code", raw: n, codeBlockStyle: "indented", text: s };
    }
  }
  fences(e) {
    let t = this.rules.block.fences.exec(e);
    if (t) {
      let n = t[0], s = Td(n, t[3] || "", this.rules);
      return { type: "code", raw: n, lang: t[2] ? t[2].trim().replace(this.rules.inline.anyPunctuation, "$1") : t[2], text: s };
    }
  }
  heading(e) {
    let t = this.rules.block.heading.exec(e);
    if (t) {
      let n = t[2].trim();
      if (this.rules.other.endingHash.test(n)) {
        let s = Ce(n, "#");
        (this.options.pedantic || !s || this.rules.other.endingSpaceChar.test(s)) && (n = s.trim());
      }
      return { type: "heading", raw: Ce(t[0], `
`), depth: t[1].length, text: n, tokens: this.lexer.inline(n) };
    }
  }
  hr(e) {
    let t = this.rules.block.hr.exec(e);
    if (t) return { type: "hr", raw: Ce(t[0], `
`) };
  }
  blockquote(e) {
    let t = this.rules.block.blockquote.exec(e);
    if (t) {
      let n = Ce(t[0], `
`).split(`
`), s = "", r = "", a = [];
      for (; n.length > 0; ) {
        let l = !1, i = [], o;
        for (o = 0; o < n.length; o++) if (this.rules.other.blockquoteStart.test(n[o])) i.push(n[o]), l = !0;
        else if (!l) i.push(n[o]);
        else break;
        n = n.slice(o);
        let d = i.join(`
`), c = d.replace(this.rules.other.blockquoteSetextReplace, `
    $1`).replace(this.rules.other.blockquoteSetextReplace2, "");
        s = s ? `${s}
${d}` : d, r = r ? `${r}
${c}` : c;
        let p = this.lexer.state.top;
        if (this.lexer.state.top = !0, this.lexer.blockTokens(c, a, !0), this.lexer.state.top = p, n.length === 0) break;
        let u = a.at(-1);
        if ((u == null ? void 0 : u.type) === "code") break;
        if ((u == null ? void 0 : u.type) === "blockquote") {
          let _ = u, f = _.raw + `
` + n.join(`
`), k = this.blockquote(f);
          a[a.length - 1] = k, s = s.substring(0, s.length - _.raw.length) + k.raw, r = r.substring(0, r.length - _.text.length) + k.text;
          break;
        } else if ((u == null ? void 0 : u.type) === "list") {
          let _ = u, f = _.raw + `
` + n.join(`
`), k = this.list(f);
          a[a.length - 1] = k, s = s.substring(0, s.length - u.raw.length) + k.raw, r = r.substring(0, r.length - _.raw.length) + k.raw, n = f.substring(a.at(-1).raw.length).split(`
`);
          continue;
        }
      }
      return { type: "blockquote", raw: s, tokens: a, text: r };
    }
  }
  list(e) {
    let t = this.rules.block.list.exec(e);
    if (t) {
      let n = t[1].trim(), s = n.length > 1, r = { type: "list", raw: "", ordered: s, start: s ? +n.slice(0, -1) : "", loose: !1, items: [] };
      n = s ? `\\d{1,9}\\${n.slice(-1)}` : `\\${n}`, this.options.pedantic && (n = s ? n : "[*+-]");
      let a = this.rules.other.listItemRegex(n), l = !1;
      for (; e; ) {
        let o = !1, d = "", c = "";
        if (!(t = a.exec(e)) || this.rules.block.hr.test(e)) break;
        d = t[0], e = e.substring(d.length);
        let p = Sd(t[2].split(`
`, 1)[0], t[1].length), u = e.split(`
`, 1)[0], _ = !p.trim(), f = 0;
        if (this.options.pedantic ? (f = 2, c = p.trimStart()) : _ ? f = t[1].length + 1 : (f = p.search(this.rules.other.nonSpaceChar), f = f > 4 ? 1 : f, c = p.slice(f), f += t[1].length), _ && this.rules.other.blankLine.test(u) && (d += u + `
`, e = e.substring(u.length + 1), o = !0), !o) {
          let k = this.rules.other.nextBulletRegex(f), y = this.rules.other.hrRegex(f), C = this.rules.other.fencesBeginRegex(f), D = this.rules.other.headingBeginRegex(f), L = this.rules.other.htmlBeginRegex(f), B = this.rules.other.blockquoteBeginRegex(f);
          for (; e; ) {
            let G = e.split(`
`, 1)[0], M;
            if (u = G, this.options.pedantic ? (u = u.replace(this.rules.other.listReplaceNesting, "  "), M = u) : M = u.replace(this.rules.other.tabCharGlobal, "    "), C.test(u) || D.test(u) || L.test(u) || B.test(u) || k.test(u) || y.test(u)) break;
            if (M.search(this.rules.other.nonSpaceChar) >= f || !u.trim()) c += `
` + M.slice(f);
            else {
              if (_ || p.replace(this.rules.other.tabCharGlobal, "    ").search(this.rules.other.nonSpaceChar) >= 4 || C.test(p) || D.test(p) || y.test(p)) break;
              c += `
` + u;
            }
            _ = !u.trim(), d += G + `
`, e = e.substring(G.length + 1), p = M.slice(f);
          }
        }
        r.loose || (l ? r.loose = !0 : this.rules.other.doubleBlankLine.test(d) && (l = !0)), r.items.push({ type: "list_item", raw: d, task: !!this.options.gfm && this.rules.other.listIsTask.test(c), loose: !1, text: c, tokens: [] }), r.raw += d;
      }
      let i = r.items.at(-1);
      if (i) i.raw = i.raw.trimEnd(), i.text = i.text.trimEnd();
      else return;
      r.raw = r.raw.trimEnd();
      for (let o of r.items) {
        this.lexer.state.top = !1, o.tokens = this.lexer.blockTokens(o.text, []);
        let d = o.tokens[0];
        if (o.task && ((d == null ? void 0 : d.type) === "text" || (d == null ? void 0 : d.type) === "paragraph")) {
          o.text = o.text.replace(this.rules.other.listReplaceTask, ""), d.raw = d.raw.replace(this.rules.other.listReplaceTask, ""), d.text = d.text.replace(this.rules.other.listReplaceTask, "");
          for (let p = this.lexer.inlineQueue.length - 1; p >= 0; p--) if (this.rules.other.listIsTask.test(this.lexer.inlineQueue[p].src)) {
            this.lexer.inlineQueue[p].src = this.lexer.inlineQueue[p].src.replace(this.rules.other.listReplaceTask, "");
            break;
          }
          let c = this.rules.other.listTaskCheckbox.exec(o.raw);
          if (c) {
            let p = { type: "checkbox", raw: c[0] + " ", checked: c[0] !== "[ ]" };
            o.checked = p.checked, r.loose ? o.tokens[0] && ["paragraph", "text"].includes(o.tokens[0].type) && "tokens" in o.tokens[0] && o.tokens[0].tokens ? (o.tokens[0].raw = p.raw + o.tokens[0].raw, o.tokens[0].text = p.raw + o.tokens[0].text, o.tokens[0].tokens.unshift(p)) : o.tokens.unshift({ type: "paragraph", raw: p.raw, text: p.raw, tokens: [p] }) : o.tokens.unshift(p);
          }
        } else o.task && (o.task = !1);
        if (!r.loose) {
          let c = o.tokens.filter((u) => u.type === "space"), p = c.length > 0 && c.some((u) => this.rules.other.anyLine.test(u.raw));
          r.loose = p;
        }
      }
      if (r.loose) for (let o of r.items) {
        o.loose = !0;
        for (let d of o.tokens) d.type === "text" && (d.type = "paragraph");
      }
      return r;
    }
  }
  html(e) {
    let t = this.rules.block.html.exec(e);
    if (t) {
      let n = wn(t[0]);
      return { type: "html", block: !0, raw: n, pre: t[1] === "pre" || t[1] === "script" || t[1] === "style", text: n };
    }
  }
  def(e) {
    let t = this.rules.block.def.exec(e);
    if (t) {
      let n = t[1].toLowerCase().replace(this.rules.other.multipleSpaceGlobal, " "), s = t[2] ? t[2].replace(this.rules.other.hrefBrackets, "$1").replace(this.rules.inline.anyPunctuation, "$1") : "", r = t[3] ? t[3].substring(1, t[3].length - 1).replace(this.rules.inline.anyPunctuation, "$1") : t[3];
      return { type: "def", tag: n, raw: Ce(t[0], `
`), href: s, title: r };
    }
  }
  table(e) {
    var l;
    let t = this.rules.block.table.exec(e);
    if (!t || !this.rules.other.tableDelimiter.test(t[2])) return;
    let n = _n(t[1]), s = t[2].replace(this.rules.other.tableAlignChars, "").split("|"), r = (l = t[3]) != null && l.trim() ? t[3].replace(this.rules.other.tableRowBlankLine, "").split(`
`) : [], a = { type: "table", raw: Ce(t[0], `
`), header: [], align: [], rows: [] };
    if (n.length === s.length) {
      for (let i of s) this.rules.other.tableAlignRight.test(i) ? a.align.push("right") : this.rules.other.tableAlignCenter.test(i) ? a.align.push("center") : this.rules.other.tableAlignLeft.test(i) ? a.align.push("left") : a.align.push(null);
      for (let i = 0; i < n.length; i++) a.header.push({ text: n[i], tokens: this.lexer.inline(n[i]), header: !0, align: a.align[i] });
      for (let i of r) a.rows.push(_n(i, a.header.length).map((o, d) => ({ text: o, tokens: this.lexer.inline(o), header: !1, align: a.align[d] })));
      return a;
    }
  }
  lheading(e) {
    let t = this.rules.block.lheading.exec(e);
    if (t) {
      let n = t[1].trim();
      return { type: "heading", raw: Ce(t[0], `
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
        let a = Ce(n.slice(0, -1), "\\");
        if ((n.length - a.length) % 2 === 0) return;
      } else {
        let a = Ed(t[2], "()");
        if (a === -2) return;
        if (a > -1) {
          let l = (t[0].indexOf("!") === 0 ? 5 : 4) + t[1].length + a;
          t[2] = t[2].substring(0, a), t[0] = t[0].substring(0, l).trim(), t[3] = "";
        }
      }
      let s = t[2], r = "";
      if (this.options.pedantic) {
        let a = this.rules.other.pedanticHrefTitle.exec(s);
        a && (s = a[1], r = a[3]);
      } else r = t[3] ? t[3].slice(1, -1) : "";
      return s = s.trim(), this.rules.other.startAngleBracket.test(s) && (this.options.pedantic && !this.rules.other.endAngleBracket.test(n) ? s = s.slice(1) : s = s.slice(1, -1)), $n(t, { href: s && s.replace(this.rules.inline.anyPunctuation, "$1"), title: r && r.replace(this.rules.inline.anyPunctuation, "$1") }, t[0], this.lexer, this.rules);
    }
  }
  reflink(e, t) {
    let n;
    if ((n = this.rules.inline.reflink.exec(e)) || (n = this.rules.inline.nolink.exec(e))) {
      let s = (n[2] || n[1]).replace(this.rules.other.multipleSpaceGlobal, " "), r = t[s.toLowerCase()];
      if (!r) {
        let a = n[0].charAt(0);
        return { type: "text", raw: a, text: a };
      }
      return $n(n, r, n[0], this.lexer, this.rules);
    }
  }
  emStrong(e, t, n = "") {
    let s = this.rules.inline.emStrongLDelim.exec(e);
    if (!(!s || !s[1] && !s[2] && !s[3] && !s[4] || s[4] && n.match(this.rules.other.unicodeAlphaNumeric)) && (!(s[1] || s[3]) || !n || this.rules.inline.punctuation.exec(n))) {
      let r = [...s[0]].length - 1, a, l, i = r, o = 0, d = s[0][0] === "*" ? this.rules.inline.emStrongRDelimAst : this.rules.inline.emStrongRDelimUnd;
      for (d.lastIndex = 0, t = t.slice(-1 * e.length + r); (s = d.exec(t)) !== null; ) {
        if (a = s[1] || s[2] || s[3] || s[4] || s[5] || s[6], !a) continue;
        if (l = [...a].length, s[3] || s[4]) {
          i += l;
          continue;
        } else if ((s[5] || s[6]) && r % 3 && !((r + l) % 3)) {
          o += l;
          continue;
        }
        if (i -= l, i > 0) continue;
        l = Math.min(l, l + i + o);
        let c = [...s[0]][0].length, p = e.slice(0, r + s.index + c + l);
        if (Math.min(r, l) % 2) {
          let _ = p.slice(1, -1);
          return { type: "em", raw: p, text: _, tokens: this.lexer.inlineTokens(_) };
        }
        let u = p.slice(2, -2);
        return { type: "strong", raw: p, text: u, tokens: this.lexer.inlineTokens(u) };
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
      let r = [...s[0]].length - 1, a, l, i = r, o = this.rules.inline.delRDelim;
      for (o.lastIndex = 0, t = t.slice(-1 * e.length + r); (s = o.exec(t)) !== null; ) {
        if (a = s[1] || s[2] || s[3] || s[4] || s[5] || s[6], !a || (l = [...a].length, l !== r)) continue;
        if (s[3] || s[4]) {
          i += l;
          continue;
        }
        if (i -= l, i > 0) continue;
        l = Math.min(l, l + i);
        let d = [...s[0]][0].length, c = e.slice(0, r + s.index + d + l), p = c.slice(r, -r);
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
        let a;
        do
          a = t[0], t[0] = ((n = this.rules.inline._backpedal.exec(t[0])) == null ? void 0 : n[0]) ?? "";
        while (a !== t[0]);
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
}, me = class It {
  constructor(t) {
    V(this, "tokens");
    V(this, "options");
    V(this, "state");
    V(this, "inlineQueue");
    V(this, "tokenizer");
    this.tokens = [], this.tokens.links = /* @__PURE__ */ Object.create(null), this.options = t || Pe, this.options.tokenizer = this.options.tokenizer || new lt(), this.tokenizer = this.options.tokenizer, this.tokenizer.options = this.options, this.tokenizer.lexer = this, this.inlineQueue = [], this.state = { inLink: !1, inRawBlock: !1, top: !0 };
    let n = { other: pe, block: nt.normal, inline: je.normal };
    this.options.pedantic ? (n.block = nt.pedantic, n.inline = je.pedantic) : this.options.gfm && (n.block = nt.gfm, this.options.breaks ? n.inline = je.breaks : n.inline = je.gfm), this.tokenizer.rules = n;
  }
  static get rules() {
    return { block: nt, inline: je };
  }
  static lex(t, n) {
    return new It(n).lex(t);
  }
  static lexInline(t, n) {
    return new It(n).inlineTokens(t);
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
    var a, l, i;
    this.tokenizer.lexer = this, this.options.pedantic && (t = t.replace(pe.tabCharGlobal, "    ").replace(pe.spaceLine, ""));
    let r = 1 / 0;
    for (; t; ) {
      if (t.length < r) r = t.length;
      else {
        this.infiniteLoopError(t.charCodeAt(0));
        break;
      }
      let o;
      if ((l = (a = this.options.extensions) == null ? void 0 : a.block) != null && l.some((c) => (o = c.call({ lexer: this }, t, n)) ? (t = t.substring(o.raw.length), n.push(o), !0) : !1)) continue;
      if (o = this.tokenizer.space(t)) {
        t = t.substring(o.raw.length);
        let c = n.at(-1);
        o.raw.length === 1 && c !== void 0 ? c.raw += `
` : n.push(o);
        continue;
      }
      if (o = this.tokenizer.code(t)) {
        t = t.substring(o.raw.length);
        let c = n.at(-1);
        (c == null ? void 0 : c.type) === "paragraph" || (c == null ? void 0 : c.type) === "text" ? (c.raw += (c.raw.endsWith(`
`) ? "" : `
`) + o.raw, c.text += `
` + o.text, this.inlineQueue.at(-1).src = c.text) : n.push(o);
        continue;
      }
      if (o = this.tokenizer.fences(t)) {
        t = t.substring(o.raw.length), n.push(o);
        continue;
      }
      if (o = this.tokenizer.heading(t)) {
        t = t.substring(o.raw.length), n.push(o);
        continue;
      }
      if (o = this.tokenizer.hr(t)) {
        t = t.substring(o.raw.length), n.push(o);
        continue;
      }
      if (o = this.tokenizer.blockquote(t)) {
        t = t.substring(o.raw.length), n.push(o);
        continue;
      }
      if (o = this.tokenizer.list(t)) {
        t = t.substring(o.raw.length), n.push(o);
        continue;
      }
      if (o = this.tokenizer.html(t)) {
        t = t.substring(o.raw.length), n.push(o);
        continue;
      }
      if (o = this.tokenizer.def(t)) {
        t = t.substring(o.raw.length);
        let c = n.at(-1);
        (c == null ? void 0 : c.type) === "paragraph" || (c == null ? void 0 : c.type) === "text" ? (c.raw += (c.raw.endsWith(`
`) ? "" : `
`) + o.raw, c.text += `
` + o.raw, this.inlineQueue.at(-1).src = c.text) : this.tokens.links[o.tag] || (this.tokens.links[o.tag] = { href: o.href, title: o.title }, n.push(o));
        continue;
      }
      if (o = this.tokenizer.table(t)) {
        t = t.substring(o.raw.length), n.push(o);
        continue;
      }
      if (o = this.tokenizer.lheading(t)) {
        t = t.substring(o.raw.length), n.push(o);
        continue;
      }
      let d = t;
      if ((i = this.options.extensions) != null && i.startBlock) {
        let c = 1 / 0, p = t.slice(1), u;
        this.options.extensions.startBlock.forEach((_) => {
          u = _.call({ lexer: this }, p), typeof u == "number" && u >= 0 && (c = Math.min(c, u));
        }), c < 1 / 0 && c >= 0 && (d = t.substring(0, c + 1));
      }
      if (this.state.top && (o = this.tokenizer.paragraph(d))) {
        let c = n.at(-1);
        s && (c == null ? void 0 : c.type) === "paragraph" ? (c.raw += (c.raw.endsWith(`
`) ? "" : `
`) + o.raw, c.text += `
` + o.text, this.inlineQueue.pop(), this.inlineQueue.at(-1).src = c.text) : n.push(o), s = d.length !== t.length, t = t.substring(o.raw.length);
        continue;
      }
      if (o = this.tokenizer.text(t)) {
        t = t.substring(o.raw.length);
        let c = n.at(-1);
        (c == null ? void 0 : c.type) === "text" ? (c.raw += (c.raw.endsWith(`
`) ? "" : `
`) + o.raw, c.text += `
` + o.text, this.inlineQueue.pop(), this.inlineQueue.at(-1).src = c.text) : n.push(o);
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
    var d, c, p, u, _;
    this.tokenizer.lexer = this;
    let s = t, r = null;
    if (this.tokens.links) {
      let f = Object.keys(this.tokens.links);
      if (f.length > 0) for (; (r = this.tokenizer.rules.inline.reflinkSearch.exec(s)) !== null; ) f.includes(r[0].slice(r[0].lastIndexOf("[") + 1, -1)) && (s = s.slice(0, r.index) + "[" + "a".repeat(r[0].length - 2) + "]" + s.slice(this.tokenizer.rules.inline.reflinkSearch.lastIndex));
    }
    for (; (r = this.tokenizer.rules.inline.anyPunctuation.exec(s)) !== null; ) s = s.slice(0, r.index) + "++" + s.slice(this.tokenizer.rules.inline.anyPunctuation.lastIndex);
    let a;
    for (; (r = this.tokenizer.rules.inline.blockSkip.exec(s)) !== null; ) a = r[2] ? r[2].length : 0, s = s.slice(0, r.index + a) + "[" + "a".repeat(r[0].length - a - 2) + "]" + s.slice(this.tokenizer.rules.inline.blockSkip.lastIndex);
    s = ((c = (d = this.options.hooks) == null ? void 0 : d.emStrongMask) == null ? void 0 : c.call({ lexer: this }, s)) ?? s;
    let l = !1, i = "", o = 1 / 0;
    for (; t; ) {
      if (t.length < o) o = t.length;
      else {
        this.infiniteLoopError(t.charCodeAt(0));
        break;
      }
      l || (i = ""), l = !1;
      let f;
      if ((u = (p = this.options.extensions) == null ? void 0 : p.inline) != null && u.some((y) => (f = y.call({ lexer: this }, t, n)) ? (t = t.substring(f.raw.length), n.push(f), !0) : !1)) continue;
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
        let y = n.at(-1);
        f.type === "text" && (y == null ? void 0 : y.type) === "text" ? (y.raw += f.raw, y.text += f.text) : n.push(f);
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
        let y = 1 / 0, C = t.slice(1), D;
        this.options.extensions.startInline.forEach((L) => {
          D = L.call({ lexer: this }, C), typeof D == "number" && D >= 0 && (y = Math.min(y, D));
        }), y < 1 / 0 && y >= 0 && (k = t.substring(0, y + 1));
      }
      if (f = this.tokenizer.inlineText(k)) {
        t = t.substring(f.raw.length), f.raw.slice(-1) !== "_" && (i = f.raw.slice(-1)), l = !0;
        let y = n.at(-1);
        (y == null ? void 0 : y.type) === "text" ? (y.raw += f.raw, y.text += f.text) : n.push(f);
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
}, ct = class {
  constructor(e) {
    V(this, "options");
    V(this, "parser");
    this.options = e || Pe;
  }
  space(e) {
    return "";
  }
  code({ text: e, lang: t, escaped: n }) {
    var a;
    let s = (a = (t || "").match(pe.notSpaceStart)) == null ? void 0 : a[0], r = e.replace(pe.endingNewline, "") + `
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
    let r = t ? "ol" : "ul", a = t && n !== 1 ? ' start="' + n + '"' : "";
    return "<" + r + a + `>
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
      let a = e.rows[r];
      n = "";
      for (let l = 0; l < a.length; l++) n += this.tablecell(a[l]);
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
    let s = this.parser.parseInline(n), r = kn(e);
    if (r === null) return s;
    e = r;
    let a = '<a href="' + e + '"';
    return t && (a += ' title="' + Ee(t) + '"'), a += ">" + s + "</a>", a;
  }
  image({ href: e, title: t, text: n, tokens: s }) {
    s && (n = this.parser.parseInline(s, this.parser.textRenderer));
    let r = kn(e);
    if (r === null) return Ee(n);
    e = r;
    let a = `<img src="${e}" alt="${Ee(n)}"`;
    return t && (a += ` title="${Ee(t)}"`), a += ">", a;
  }
  text(e) {
    return "tokens" in e && e.tokens ? this.parser.parseInline(e.tokens) : "escaped" in e && e.escaped ? e.text : Ee(e.text);
  }
}, Zt = class {
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
}, ke = class Ot {
  constructor(t) {
    V(this, "options");
    V(this, "renderer");
    V(this, "textRenderer");
    this.options = t || Pe, this.options.renderer = this.options.renderer || new ct(), this.renderer = this.options.renderer, this.renderer.options = this.options, this.renderer.parser = this, this.textRenderer = new Zt();
  }
  static parse(t, n) {
    return new Ot(n).parse(t);
  }
  static parseInline(t, n) {
    return new Ot(n).parseInline(t);
  }
  parse(t) {
    var s, r;
    this.renderer.parser = this;
    let n = "";
    for (let a = 0; a < t.length; a++) {
      let l = t[a];
      if ((r = (s = this.options.extensions) == null ? void 0 : s.renderers) != null && r[l.type]) {
        let o = l, d = this.options.extensions.renderers[o.type].call({ parser: this }, o);
        if (d !== !1 || !["space", "hr", "heading", "code", "table", "blockquote", "list", "html", "def", "paragraph", "text"].includes(o.type)) {
          n += d || "";
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
          let o = 'Token with "' + i.type + '" type was not found.';
          if (this.options.silent) return console.error(o), "";
          throw new Error(o);
        }
      }
    }
    return n;
  }
  parseInline(t, n = this.renderer) {
    var r, a;
    this.renderer.parser = this;
    let s = "";
    for (let l = 0; l < t.length; l++) {
      let i = t[l];
      if ((a = (r = this.options.extensions) == null ? void 0 : r.renderers) != null && a[i.type]) {
        let d = this.options.extensions.renderers[i.type].call({ parser: this }, i);
        if (d !== !1 || !["escape", "html", "link", "image", "strong", "em", "codespan", "br", "del", "text"].includes(i.type)) {
          s += d || "";
          continue;
        }
      }
      let o = i;
      switch (o.type) {
        case "escape": {
          s += n.text(o);
          break;
        }
        case "html": {
          s += n.html(o);
          break;
        }
        case "link": {
          s += n.link(o);
          break;
        }
        case "image": {
          s += n.image(o);
          break;
        }
        case "checkbox": {
          s += n.checkbox(o);
          break;
        }
        case "strong": {
          s += n.strong(o);
          break;
        }
        case "em": {
          s += n.em(o);
          break;
        }
        case "codespan": {
          s += n.codespan(o);
          break;
        }
        case "br": {
          s += n.br(o);
          break;
        }
        case "del": {
          s += n.del(o);
          break;
        }
        case "text": {
          s += n.text(o);
          break;
        }
        default: {
          let d = 'Token with "' + o.type + '" type was not found.';
          if (this.options.silent) return console.error(d), "";
          throw new Error(d);
        }
      }
    }
    return s;
  }
}, st, We = (st = class {
  constructor(e) {
    V(this, "options");
    V(this, "block");
    this.options = e || Pe;
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
}, V(st, "passThroughHooks", /* @__PURE__ */ new Set(["preprocess", "postprocess", "processAllTokens", "emStrongMask"])), V(st, "passThroughHooksRespectAsync", /* @__PURE__ */ new Set(["preprocess", "postprocess", "processAllTokens"])), st), ws = class {
  constructor(...e) {
    V(this, "defaults", qt());
    V(this, "options", this.setOptions);
    V(this, "parse", this.parseMarkdown(!0));
    V(this, "parseInline", this.parseMarkdown(!1));
    V(this, "Parser", ke);
    V(this, "Renderer", ct);
    V(this, "TextRenderer", Zt);
    V(this, "Lexer", me);
    V(this, "Tokenizer", lt);
    V(this, "Hooks", We);
    this.use(...e);
  }
  walkTokens(e, t) {
    var s, r;
    let n = [];
    for (let a of e) switch (n = n.concat(t.call(this, a)), a.type) {
      case "table": {
        let l = a;
        for (let i of l.header) n = n.concat(this.walkTokens(i.tokens, t));
        for (let i of l.rows) for (let o of i) n = n.concat(this.walkTokens(o.tokens, t));
        break;
      }
      case "list": {
        let l = a;
        n = n.concat(this.walkTokens(l.items, t));
        break;
      }
      default: {
        let l = a;
        (r = (s = this.defaults.extensions) == null ? void 0 : s.childTokens) != null && r[l.type] ? this.defaults.extensions.childTokens[l.type].forEach((i) => {
          let o = l[i].flat(1 / 0);
          n = n.concat(this.walkTokens(o, t));
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
          let a = t.renderers[r.name];
          a ? t.renderers[r.name] = function(...l) {
            let i = r.renderer.apply(this, l);
            return i === !1 && (i = a.apply(this, l)), i;
          } : t.renderers[r.name] = r.renderer;
        }
        if ("tokenizer" in r) {
          if (!r.level || r.level !== "block" && r.level !== "inline") throw new Error("extension level must be 'block' or 'inline'");
          let a = t[r.level];
          a ? a.unshift(r.tokenizer) : t[r.level] = [r.tokenizer], r.start && (r.level === "block" ? t.startBlock ? t.startBlock.push(r.start) : t.startBlock = [r.start] : r.level === "inline" && (t.startInline ? t.startInline.push(r.start) : t.startInline = [r.start]));
        }
        "childTokens" in r && r.childTokens && (t.childTokens[r.name] = r.childTokens);
      }), s.extensions = t), n.renderer) {
        let r = this.defaults.renderer || new ct(this.defaults);
        for (let a in n.renderer) {
          if (!(a in r)) throw new Error(`renderer '${a}' does not exist`);
          if (["options", "parser"].includes(a)) continue;
          let l = a, i = n.renderer[l], o = r[l];
          r[l] = (...d) => {
            let c = i.apply(r, d);
            return c === !1 && (c = o.apply(r, d)), c || "";
          };
        }
        s.renderer = r;
      }
      if (n.tokenizer) {
        let r = this.defaults.tokenizer || new lt(this.defaults);
        for (let a in n.tokenizer) {
          if (!(a in r)) throw new Error(`tokenizer '${a}' does not exist`);
          if (["options", "rules", "lexer"].includes(a)) continue;
          let l = a, i = n.tokenizer[l], o = r[l];
          r[l] = (...d) => {
            let c = i.apply(r, d);
            return c === !1 && (c = o.apply(r, d)), c;
          };
        }
        s.tokenizer = r;
      }
      if (n.hooks) {
        let r = this.defaults.hooks || new We();
        for (let a in n.hooks) {
          if (!(a in r)) throw new Error(`hook '${a}' does not exist`);
          if (["options", "block"].includes(a)) continue;
          let l = a, i = n.hooks[l], o = r[l];
          We.passThroughHooks.has(a) ? r[l] = (d) => {
            if (this.defaults.async && We.passThroughHooksRespectAsync.has(a)) return (async () => {
              let p = await i.call(r, d);
              return o.call(r, p);
            })();
            let c = i.call(r, d);
            return o.call(r, c);
          } : r[l] = (...d) => {
            if (this.defaults.async) return (async () => {
              let p = await i.apply(r, d);
              return p === !1 && (p = await o.apply(r, d)), p;
            })();
            let c = i.apply(r, d);
            return c === !1 && (c = o.apply(r, d)), c;
          };
        }
        s.hooks = r;
      }
      if (n.walkTokens) {
        let r = this.defaults.walkTokens, a = n.walkTokens;
        s.walkTokens = function(l) {
          let i = [];
          return i.push(a.call(this, l)), r && (i = i.concat(r.call(this, l))), i;
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
      let s = { ...n }, r = { ...this.defaults, ...s }, a = this.onError(!!r.silent, !!r.async);
      if (this.defaults.async === !0 && s.async === !1) return a(new Error("marked(): The async option was set to true by an extension. Remove async: false from the parse options object to return a Promise."));
      if (typeof t > "u" || t === null) return a(new Error("marked(): input parameter is undefined or null"));
      if (typeof t != "string") return a(new Error("marked(): input parameter is of type " + Object.prototype.toString.call(t) + ", string expected"));
      if (r.hooks && (r.hooks.options = r, r.hooks.block = e), r.async) return (async () => {
        let l = r.hooks ? await r.hooks.preprocess(t) : t, i = await (r.hooks ? await r.hooks.provideLexer(e) : e ? me.lex : me.lexInline)(l, r), o = r.hooks ? await r.hooks.processAllTokens(i) : i;
        r.walkTokens && await Promise.all(this.walkTokens(o, r.walkTokens));
        let d = await (r.hooks ? await r.hooks.provideParser(e) : e ? ke.parse : ke.parseInline)(o, r);
        return r.hooks ? await r.hooks.postprocess(d) : d;
      })().catch(a);
      try {
        r.hooks && (t = r.hooks.preprocess(t));
        let l = (r.hooks ? r.hooks.provideLexer(e) : e ? me.lex : me.lexInline)(t, r);
        r.hooks && (l = r.hooks.processAllTokens(l)), r.walkTokens && this.walkTokens(l, r.walkTokens);
        let i = (r.hooks ? r.hooks.provideParser(e) : e ? ke.parse : ke.parseInline)(l, r);
        return r.hooks && (i = r.hooks.postprocess(i)), i;
      } catch (l) {
        return a(l);
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
}, Be = new ws();
function J(e, t) {
  return Be.parse(e, t);
}
J.options = J.setOptions = function(e) {
  return Be.setOptions(e), J.defaults = Be.defaults, us(J.defaults), J;
};
J.getDefaults = qt;
J.defaults = Pe;
J.use = function(...e) {
  return Be.use(...e), J.defaults = Be.defaults, us(J.defaults), J;
};
J.walkTokens = function(e, t) {
  return Be.walkTokens(e, t);
};
J.parseInline = Be.parseInline;
J.Parser = ke;
J.parser = ke.parse;
J.Renderer = ct;
J.TextRenderer = Zt;
J.Lexer = me;
J.lexer = me.lex;
J.Tokenizer = lt;
J.Hooks = We;
J.parse = J;
J.options;
J.setOptions;
J.use;
J.walkTokens;
J.parseInline;
ke.parse;
me.lex;
const xd = ["innerHTML"], bm = /* @__PURE__ */ z({
  __name: "Markdown",
  props: {
    content: {}
  },
  setup(e) {
    const t = e, n = new ws({ gfm: !0, breaks: !1 });
    n.use({
      renderer: {
        code(a) {
          const l = a.text, i = (a.lang || "").replace(/[^a-z0-9_-]/gi, ""), o = i && oe.getLanguage(i) ? i : "", d = o ? oe.highlight(l, { language: o, ignoreIllegals: !0 }).value : oe.highlightAuto(l).value, c = o ? ` data-lang="${o}"` : "";
          return `<div class="md-code"><div class="md-code-bar"><span class="md-lang">${o || "text"}</span><button type="button" class="md-copy" aria-label="Copy code">Copy</button></div><pre${c}><code class="hljs">${d}</code></pre></div>`;
        }
      }
    });
    const s = Y(() => n.parse(t.content));
    async function r(a) {
      var o;
      const l = a.target.closest(".md-copy");
      if (!l) return;
      const i = (o = l.closest(".md-code")) == null ? void 0 : o.querySelector("code");
      if (i)
        try {
          await navigator.clipboard.writeText(i.textContent ?? "");
          const d = l.textContent;
          l.textContent = "Copied", l.classList.add("ok"), setTimeout(() => {
            l.textContent = d, l.classList.remove("ok");
          }, 1200);
        } catch {
        }
    }
    return (a, l) => (g(), b("div", {
      class: "hive-md",
      innerHTML: s.value,
      onClick: r
    }, null, 8, xd));
  }
}), Md = /* @__PURE__ */ z({
  __name: "NewBadge",
  props: {
    variant: { default: "new" }
  },
  setup(e) {
    return (t, n) => (g(), b("span", {
      class: h([t.$style.badge, t.$style[e.variant]])
    }, [
      Q(t.$slots, "default", {}, () => [
        fe(S(e.variant), 1)
      ])
    ], 2));
  }
}), Cd = "_badge_13w2z_2", Ad = "_draft_13w2z_21", Id = "_beta_13w2z_26", Od = "_todo_13w2z_31", Nd = {
  badge: Cd,
  new: "_new_13w2z_16",
  draft: Ad,
  beta: Id,
  todo: Od
}, Rd = {
  $style: Nd
}, vn = /* @__PURE__ */ P(Md, [["__cssModules", Rd]]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Ld = (e) => {
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
const En = (e) => e === "";
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Bd = (...e) => e.filter((t, n, s) => !!t && t.trim() !== "" && s.indexOf(t) === n).join(" ").trim();
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Sn = (e) => e.replace(/([a-z0-9])([A-Z])/g, "$1-$2").toLowerCase();
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const zd = (e) => e.replace(
  /^([A-Z])|[\s-_]+(\w)/g,
  (t, n, s) => s ? s.toUpperCase() : n.toLowerCase()
);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Pd = (e) => {
  const t = zd(e);
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
const Dd = ({
  name: e,
  iconNode: t,
  absoluteStrokeWidth: n,
  "absolute-stroke-width": s,
  strokeWidth: r,
  "stroke-width": a,
  size: l = He.width,
  color: i = He.stroke,
  ...o
}, { slots: d }) => Ct(
  "svg",
  {
    ...He,
    ...o,
    width: l,
    height: l,
    stroke: i,
    "stroke-width": En(n) || En(s) || n === !0 || s === !0 ? Number(r || a || He["stroke-width"]) * 24 / Number(l) : r || a || He["stroke-width"],
    class: Bd(
      "lucide",
      o.class,
      ...e ? [`lucide-${Sn(Pd(e))}-icon`, `lucide-${Sn(e)}`] : ["lucide-icon"]
    ),
    ...!d.default && !Ld(o) && { "aria-hidden": "true" }
  },
  [...t.map((c) => Ct(...c)), ...d.default ? [d.default()] : []]
);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const H = (e, t) => (n, { slots: s, attrs: r }) => Ct(
  Dd,
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
const Ud = H("arrow-right", [
  ["path", { d: "M5 12h14", key: "1ays0h" }],
  ["path", { d: "m12 5 7 7-7 7", key: "xquz4c" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const qd = H("bot", [
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
const jd = H("brain", [
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
const Hd = H("check", [["path", { d: "M20 6 9 17l-5-5", key: "1gmf2c" }]]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Vd = H("chevron-down", [
  ["path", { d: "m6 9 6 6 6-6", key: "qrunsl" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Fd = H("chevron-left", [
  ["path", { d: "m15 18-6-6 6-6", key: "1wnfg3" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Gd = H("chevron-right", [
  ["path", { d: "m9 18 6-6-6-6", key: "mthhwq" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Wd = H("chevron-up", [
  ["path", { d: "m18 15-6-6-6 6", key: "153udz" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Kd = H("circle-alert", [
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
const Zd = H("circle-check", [
  ["circle", { cx: "12", cy: "12", r: "10", key: "1mglay" }],
  ["path", { d: "m9 12 2 2 4-4", key: "dzmm74" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Xd = H("circle-x", [
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
const Yd = H("clock", [
  ["circle", { cx: "12", cy: "12", r: "10", key: "1mglay" }],
  ["path", { d: "M12 6v6l4 2", key: "mmk7yg" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Jd = H("copy", [
  ["rect", { width: "14", height: "14", x: "8", y: "8", rx: "2", ry: "2", key: "17jyea" }],
  ["path", { d: "M4 16c-1.1 0-2-.9-2-2V4c0-1.1.9-2 2-2h10c1.1 0 2 .9 2 2", key: "zix9uf" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Qd = H("cpu", [
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
const eu = H("database", [
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
const tu = H("ellipsis", [
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
const nu = H("external-link", [
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
const su = H("eye", [
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
const ru = H("file-text", [
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
const au = H("funnel", [
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
const ou = H("hash", [
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
const iu = H("history", [
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
const lu = H("image", [
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
const cu = H("info", [
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
const du = H("key-round", [
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
const uu = H("loader-circle", [
  ["path", { d: "M21 12a9 9 0 1 1-6.219-8.56", key: "13zald" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const hu = H("menu", [
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
const pu = H("message-square", [
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
const fu = H("network", [
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
const gu = H("pause", [
  ["rect", { x: "14", y: "3", width: "5", height: "18", rx: "1", key: "kaeet6" }],
  ["rect", { x: "5", y: "3", width: "5", height: "18", rx: "1", key: "1wsw3u" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const yu = H("pencil", [
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
const bu = H("play", [
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
const mu = H("plus", [
  ["path", { d: "M5 12h14", key: "1ays0h" }],
  ["path", { d: "M12 5v14", key: "s699le" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const ku = H("refresh-cw", [
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
const _u = H("search", [
  ["path", { d: "m21 21-4.34-4.34", key: "14j7rj" }],
  ["circle", { cx: "11", cy: "11", r: "8", key: "4ej97u" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const wu = H("send", [
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
const $u = H("settings", [
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
const vu = H("share-2", [
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
const Eu = H("trash-2", [
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
const Su = H("triangle-alert", [
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
const Tu = H("wrench", [
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
const xu = H("x", [
  ["path", { d: "M18 6 6 18", key: "1bl5f8" }],
  ["path", { d: "m6 6 12 12", key: "d8bk6v" }]
]);
/**
 * @license lucide-vue-next v1.0.0 - ISC
 *
 * This source code is licensed under the ISC license.
 * See the LICENSE file in the root directory of this source tree.
 */
const Mu = H("zap", [
  [
    "path",
    {
      d: "M4 14a1 1 0 0 1-.78-1.63l9.9-10.2a.5.5 0 0 1 .86.46l-1.92 6.02A1 1 0 0 0 13 10h7a1 1 0 0 1 .78 1.63l-9.9 10.2a.5.5 0 0 1-.86-.46l1.92-6.02A1 1 0 0 0 11 14z",
      key: "1xq2db"
    }
  ]
]), $s = {
  // actions
  plus: mu,
  search: _u,
  close: xu,
  check: Hd,
  edit: yu,
  trash: Eu,
  copy: Jd,
  refresh: ku,
  settings: $u,
  filter: au,
  more: tu,
  menu: hu,
  send: wu,
  play: bu,
  pause: gu,
  external: nu,
  // navigation
  "chevron-right": Gd,
  "chevron-down": Vd,
  "chevron-left": Fd,
  "chevron-up": Wd,
  "arrow-right": Ud,
  // hive domain
  brain: jd,
  database: eu,
  wrench: Tu,
  eye: su,
  image: lu,
  bolt: Mu,
  network: fu,
  share: vu,
  key: du,
  file: ru,
  hash: ou,
  bot: qd,
  cpu: Qd,
  chat: pu,
  clock: Yd,
  history: iu,
  // status
  ok: Zd,
  warn: Su,
  error: Xd,
  info: cu,
  alert: Kd,
  loader: uu
}, mm = Object.keys($s), Cu = /* @__PURE__ */ z({
  __name: "Icon",
  props: {
    name: {},
    size: { default: 16 },
    stroke: { default: 1.75 }
  },
  setup(e) {
    return (t, n) => (g(), ce(sr(rt($s)[e.name]), {
      size: e.size,
      "stroke-width": e.stroke,
      class: h(t.$style.icon),
      "aria-hidden": "true"
    }, null, 8, ["size", "stroke-width", "class"]));
  }
}), Au = "_icon_18v37_2", Iu = {
  icon: Au
}, Ou = {
  $style: Iu
}, vs = /* @__PURE__ */ P(Cu, [["__cssModules", Ou]]), Nu = /* @__PURE__ */ z({
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
    function r(a) {
      n.open && n.closeOnEsc && a.key === "Escape" && s("close");
    }
    return dt(() => n.open, (a) => {
      a ? window.addEventListener("keydown", r) : window.removeEventListener("keydown", r);
    }, { immediate: !0 }), Lt(() => window.removeEventListener("keydown", r)), (a, l) => (g(), ce(ut, { to: "body" }, [
      e.open ? (g(), b("div", {
        key: 0,
        class: h(a.$style.overlay),
        onClick: l[1] || (l[1] = zn((i) => e.closeOnBackdrop && s("close"), ["self"]))
      }, [
        $("div", {
          class: h([a.$style.panel, a.$style[e.size]]),
          role: "dialog",
          "aria-modal": "true"
        }, [
          e.title || a.$slots.header ? (g(), b("header", {
            key: 0,
            class: h(a.$style.head)
          }, [
            Q(a.$slots, "header", {}, () => [
              $("span", {
                class: h(a.$style.title)
              }, S(e.title), 3)
            ]),
            $("button", {
              type: "button",
              class: h(a.$style.close),
              "aria-label": "Close",
              onClick: l[0] || (l[0] = (i) => s("close"))
            }, "×", 2)
          ], 2)) : N("", !0),
          $("div", {
            class: h(a.$style.body)
          }, [
            Q(a.$slots, "default")
          ], 2),
          a.$slots.footer ? (g(), b("footer", {
            key: 1,
            class: h(a.$style.foot)
          }, [
            Q(a.$slots, "footer")
          ], 2)) : N("", !0)
        ], 2)
      ], 2)) : N("", !0)
    ]));
  }
}), Ru = "_overlay_1yxgf_2", Lu = "_panel_1yxgf_3", Bu = "_sm_1yxgf_4", zu = "_md_1yxgf_4", Pu = "_lg_1yxgf_4", Du = "_head_1yxgf_5", Uu = "_title_1yxgf_6", qu = "_close_1yxgf_7", ju = "_body_1yxgf_9", Hu = "_foot_1yxgf_10", Vu = {
  overlay: Ru,
  panel: Lu,
  sm: Bu,
  md: zu,
  lg: Pu,
  head: Du,
  title: Uu,
  close: qu,
  body: ju,
  foot: Hu
}, Fu = {
  $style: Vu
}, Gu = /* @__PURE__ */ P(Nu, [["__cssModules", Fu]]), Wu = /* @__PURE__ */ z({
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
    return (s, r) => (g(), ce(Gu, {
      open: e.open,
      title: e.title,
      size: "sm",
      onClose: r[2] || (r[2] = (a) => n("cancel"))
    }, {
      footer: be(() => [
        $("button", {
          type: "button",
          class: h(s.$style.cancel),
          onClick: r[0] || (r[0] = (a) => n("cancel"))
        }, S(e.cancelText), 3),
        $("button", {
          type: "button",
          class: h([s.$style.confirm, e.danger && s.$style.danger]),
          onClick: r[1] || (r[1] = (a) => n("confirm"))
        }, S(e.confirmText), 3)
      ]),
      default: be(() => [
        $("p", {
          class: h(s.$style.msg)
        }, [
          Q(s.$slots, "default", {}, () => [
            fe(S(e.message), 1)
          ])
        ], 2)
      ]),
      _: 3
    }, 8, ["open", "title"]));
  }
}), Ku = "_msg_ad9ml_2", Zu = "_cancel_ad9ml_3", Xu = "_confirm_ad9ml_3", Yu = "_danger_ad9ml_7", Ju = {
  msg: Ku,
  cancel: Zu,
  confirm: Xu,
  danger: Yu
}, Qu = {
  $style: Ju
}, km = /* @__PURE__ */ P(Wu, [["__cssModules", Qu]]), eh = /* @__PURE__ */ z({
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
    function r(a) {
      n.open && a.key === "Escape" && s("close");
    }
    return dt(() => n.open, (a) => {
      a ? window.addEventListener("keydown", r) : window.removeEventListener("keydown", r);
    }, { immediate: !0 }), Lt(() => window.removeEventListener("keydown", r)), (a, l) => (g(), ce(ut, { to: "body" }, [
      e.open ? (g(), b("div", {
        key: 0,
        class: h(a.$style.overlay),
        onClick: l[1] || (l[1] = zn((i) => s("close"), ["self"]))
      }, [
        $("aside", {
          class: h([a.$style.drawer, a.$style[e.side]]),
          style: Se({ width: e.width + "px" }),
          role: "dialog",
          "aria-modal": "true"
        }, [
          e.title || a.$slots.header ? (g(), b("header", {
            key: 0,
            class: h(a.$style.head)
          }, [
            Q(a.$slots, "header", {}, () => [
              $("span", {
                class: h(a.$style.title)
              }, S(e.title), 3)
            ]),
            $("button", {
              type: "button",
              class: h(a.$style.close),
              "aria-label": "Close",
              onClick: l[0] || (l[0] = (i) => s("close"))
            }, "×", 2)
          ], 2)) : N("", !0),
          $("div", {
            class: h(a.$style.body)
          }, [
            Q(a.$slots, "default")
          ], 2),
          a.$slots.footer ? (g(), b("footer", {
            key: 1,
            class: h(a.$style.foot)
          }, [
            Q(a.$slots, "footer")
          ], 2)) : N("", !0)
        ], 6)
      ], 2)) : N("", !0)
    ]));
  }
}), th = "_overlay_1r4yl_2", nh = "_drawer_1r4yl_3", sh = "_right_1r4yl_4", rh = "_left_1r4yl_5", ah = "_head_1r4yl_6", oh = "_title_1r4yl_7", ih = "_close_1r4yl_8", lh = "_body_1r4yl_10", ch = "_foot_1r4yl_11", dh = {
  overlay: th,
  drawer: nh,
  right: sh,
  left: rh,
  head: ah,
  title: oh,
  close: ih,
  body: lh,
  foot: ch
}, uh = {
  $style: dh
}, _m = /* @__PURE__ */ P(eh, [["__cssModules", uh]]), Ye = re([]);
let hh = 0;
function Ve(e, t = "info", n = 4e3) {
  const s = ++hh;
  return Ye.value = [...Ye.value, { id: s, tone: t, message: e }], n > 0 && setTimeout(() => Es(s), n), s;
}
function Es(e) {
  Ye.value = Ye.value.filter((t) => t.id !== e);
}
function ph() {
  return {
    toasts: Ye,
    push: Ve,
    dismiss: Es,
    info: (e, t) => Ve(e, "info", t),
    success: (e, t) => Ve(e, "success", t),
    warn: (e, t) => Ve(e, "warn", t),
    error: (e, t) => Ve(e, "error", t ?? 6e3)
  };
}
const fh = ["onClick"], gh = /* @__PURE__ */ z({
  __name: "ToastHost",
  setup(e) {
    const { toasts: t, dismiss: n } = ph();
    return (s, r) => (g(), ce(ut, { to: "body" }, [
      $("div", {
        class: h(s.$style.host)
      }, [
        (g(!0), b(le, null, ye(rt(t), (a) => (g(), b("div", {
          key: a.id,
          class: h([s.$style.toast, s.$style[a.tone]]),
          role: "status",
          onClick: (l) => rt(n)(a.id)
        }, S(a.message), 11, fh))), 128))
      ], 2)
    ]));
  }
}), yh = "_host_45iih_2", bh = "_toast_45iih_3", mh = "_info_45iih_4", kh = "_success_45iih_5", _h = "_error_45iih_6", wh = "_warn_45iih_7", $h = {
  host: yh,
  toast: bh,
  info: mh,
  success: kh,
  error: _h,
  warn: wh
}, vh = {
  $style: $h
}, wm = /* @__PURE__ */ P(gh, [["__cssModules", vh]]), Eh = ["value", "disabled"], Sh = {
  key: 0,
  value: "",
  disabled: ""
}, Th = ["value", "disabled"], xh = /* @__PURE__ */ z({
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
      $("select", {
        class: h(s.$style.select),
        value: e.modelValue,
        disabled: e.disabled,
        onChange: r[0] || (r[0] = (a) => n("update:modelValue", a.target.value))
      }, [
        e.placeholder ? (g(), b("option", Sh, S(e.placeholder), 1)) : N("", !0),
        (g(!0), b(le, null, ye(e.options, (a) => (g(), b("option", {
          key: a.value,
          value: a.value,
          disabled: a.disabled
        }, S(a.label), 9, Th))), 128))
      ], 42, Eh),
      $("span", {
        class: h(s.$style.arrow)
      }, "▾", 2)
    ], 2));
  }
}), Mh = "_wrap_32hoq_2", Ch = "_select_32hoq_3", Ah = "_sm_32hoq_4", Ih = "_md_32hoq_5", Oh = "_arrow_32hoq_8", Nh = {
  wrap: Mh,
  select: Ch,
  sm: Ah,
  md: Ih,
  arrow: Oh
}, Rh = {
  $style: Nh
}, Lh = /* @__PURE__ */ P(xh, [["__cssModules", Rh]]), Bh = /* @__PURE__ */ z({
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
    const n = e, s = t, r = re([]), a = re({}), l = re(!1), i = re(""), o = re(!1);
    async function d() {
      if (!n.connectionId && !n.baseUrl) {
        r.value = [], a.value = {};
        return;
      }
      l.value = !0, i.value = "";
      try {
        if (n.connectionId) {
          const y = await n.spine.listProviderModelInfo(n.connectionId);
          r.value = y.map((C) => C.id), a.value = Object.fromEntries(y.map((C) => [C.id, C]));
        } else
          r.value = await n.spine.fetchModelIds(n.baseUrl, n.apiKey), a.value = {};
      } catch (y) {
        i.value = y.message || "provider unreachable", r.value = [], a.value = {};
      } finally {
        l.value = !1;
      }
    }
    function c(y) {
      return y >= 1e6 ? `${(y / 1e6).toFixed(y % 1e6 === 0 ? 0 : 1)}M` : `${Math.round(y / 1e3)}k`;
    }
    function p(y) {
      if (!y) return "";
      const C = [];
      return y.contextLength && C.push(`${c(y.contextLength)} ctx`), y.inputPrice != null && y.outputPrice != null && C.push(`$${y.inputPrice}/$${y.outputPrice}`), C.length ? `  —  ${C.join(" · ")}` : "";
    }
    const u = Y(() => a.value[n.modelValue]);
    rr(d), dt(() => [n.connectionId, n.baseUrl, n.apiKey], d);
    const _ = Y(() => {
      const y = r.value.map((C) => ({ label: C + p(a.value[C]), value: C }));
      return n.modelValue && !r.value.includes(n.modelValue) && y.unshift({ label: `${n.modelValue} (not in list)`, value: n.modelValue }), y;
    }), f = Y(() => o.value || !!i.value && r.value.length === 0), k = (y) => s("update:modelValue", y);
    return (y, C) => (g(), b("div", {
      class: h(y.$style.wrap)
    }, [
      f.value ? (g(), ce(ua, {
        key: 0,
        modelValue: e.modelValue,
        disabled: e.disabled,
        placeholder: "model id",
        "onUpdate:modelValue": k
      }, null, 8, ["modelValue", "disabled"])) : (g(), ce(Lh, {
        key: 1,
        modelValue: e.modelValue,
        options: _.value,
        placeholder: l.value ? "Loading models…" : e.placeholder,
        disabled: e.disabled || l.value,
        size: e.size,
        "onUpdate:modelValue": k
      }, null, 8, ["modelValue", "options", "placeholder", "disabled", "size"])),
      u.value && !f.value ? (g(), b("div", {
        key: 2,
        class: h(y.$style.stats)
      }, [
        u.value.type ? (g(), b("span", {
          key: 0,
          class: h(y.$style.stat)
        }, S(u.value.type), 3)) : N("", !0),
        u.value.contextLength ? (g(), b("span", {
          key: 1,
          class: h(y.$style.stat)
        }, S(c(u.value.contextLength)) + " ctx", 3)) : N("", !0),
        u.value.maxOutput ? (g(), b("span", {
          key: 2,
          class: h(y.$style.stat)
        }, S(c(u.value.maxOutput)) + " out", 3)) : N("", !0),
        u.value.inputPrice != null && u.value.outputPrice != null ? (g(), b("span", {
          key: 3,
          class: h(y.$style.stat)
        }, " $" + S(u.value.inputPrice) + "/$" + S(u.value.outputPrice) + " per 1M ", 3)) : N("", !0)
      ], 2)) : N("", !0),
      $("div", {
        class: h(y.$style.foot)
      }, [
        i.value ? (g(), b("span", {
          key: 0,
          class: h(y.$style.err)
        }, "⚠ " + S(i.value), 3)) : N("", !0),
        $("button", {
          type: "button",
          class: h(y.$style.toggle),
          onClick: C[0] || (C[0] = (D) => o.value = !o.value)
        }, S(f.value ? "pick from list" : "custom id"), 3)
      ], 2)
    ], 2));
  }
}), zh = "_wrap_1s6sq_2", Ph = "_stats_1s6sq_3", Dh = "_stat_1s6sq_3", Uh = "_foot_1s6sq_9", qh = "_err_1s6sq_10", jh = "_toggle_1s6sq_11", Hh = {
  wrap: zh,
  stats: Ph,
  stat: Dh,
  foot: Uh,
  err: qh,
  toggle: jh
}, Vh = {
  $style: Hh
}, $m = /* @__PURE__ */ P(Bh, [["__cssModules", Vh]]), Fh = ["value", "placeholder"], Gh = /* @__PURE__ */ z({
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
    return (r, a) => (g(), b("div", {
      class: h([r.$style.wrap, r.$style[e.size]])
    }, [
      Ae(vs, {
        name: "search",
        size: 15,
        class: h(r.$style.icon)
      }, null, 8, ["class"]),
      $("input", {
        class: h(r.$style.input),
        type: "search",
        value: e.modelValue,
        placeholder: e.placeholder,
        onInput: a[0] || (a[0] = (l) => n("update:modelValue", l.target.value))
      }, null, 42, Fh),
      e.modelValue ? (g(), b("button", {
        key: 0,
        type: "button",
        class: h(r.$style.clear),
        "aria-label": "Clear",
        onClick: s
      }, "×", 2)) : N("", !0)
    ], 2));
  }
}), Wh = "_wrap_1h6ub_2", Kh = "_icon_1h6ub_3", Zh = "_input_1h6ub_4", Xh = "_sm_1h6ub_5", Yh = "_md_1h6ub_6", Jh = "_clear_1h6ub_9", Qh = {
  wrap: Wh,
  icon: Kh,
  input: Zh,
  sm: Xh,
  md: Yh,
  clear: Jh
}, ep = {
  $style: Qh
}, vm = /* @__PURE__ */ P(Gh, [["__cssModules", ep]]), tp = ["title"], np = { key: 0 }, sp = /* @__PURE__ */ z({
  __name: "CopyButton",
  props: {
    value: {},
    label: {},
    size: { default: "sm" }
  },
  setup(e) {
    const t = e, n = re(!1);
    async function s() {
      var r;
      try {
        await ((r = navigator.clipboard) == null ? void 0 : r.writeText(t.value)), n.value = !0, setTimeout(() => n.value = !1, 1500);
      } catch {
      }
    }
    return (r, a) => (g(), b("button", {
      type: "button",
      class: h([r.$style.btn, r.$style[e.size]]),
      title: e.label ?? "Copy",
      onClick: s
    }, [
      Ae(vs, {
        name: n.value ? "check" : "copy",
        size: 14
      }, null, 8, ["name"]),
      e.label || n.value ? (g(), b("span", np, S(n.value ? "Copied" : e.label), 1)) : N("", !0)
    ], 10, tp));
  }
}), rp = "_btn_1mypx_2", ap = "_sm_1mypx_3", op = "_md_1mypx_4", ip = {
  btn: rp,
  sm: ap,
  md: op
}, lp = {
  $style: ip
}, Em = /* @__PURE__ */ P(sp, [["__cssModules", lp]]), cp = /* @__PURE__ */ z({
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
    return (r, a) => (g(), b("span", {
      class: h(r.$style.wrap)
    }, [
      $("span", {
        class: h([r.$style.dot, r.$style[s.value]])
      }, null, 2),
      $("span", {
        class: h(r.$style.label)
      }, S(e.label ?? e.status), 3)
    ], 2));
  }
}), dp = "_wrap_1jn39_2", up = "_dot_1jn39_3", hp = "_ok_1jn39_4", pp = "_warn_1jn39_5", fp = "_err_1jn39_6", gp = "_run_1jn39_7", yp = "_muted_1jn39_8", bp = "_label_1jn39_9", mp = {
  wrap: dp,
  dot: up,
  ok: hp,
  warn: pp,
  err: fp,
  run: gp,
  muted: yp,
  label: bp
}, kp = {
  $style: mp
}, Sm = /* @__PURE__ */ P(cp, [["__cssModules", kp]]), _p = { key: 0 }, wp = /* @__PURE__ */ z({
  __name: "JsonViewer",
  props: {
    value: {},
    name: {},
    depth: { default: 0 },
    defaultOpen: { type: Boolean, default: !0 }
  },
  setup(e) {
    const t = e, n = Y(() => t.value !== null && typeof t.value == "object"), s = Y(() => Array.isArray(t.value)), r = Y(() => n.value ? s.value ? t.value.map((d, c) => [String(c), d]) : Object.entries(t.value) : []), a = Y(() => s.value ? `[${r.value.length}]` : `{${r.value.length}}`), l = re(t.defaultOpen && t.depth < 2);
    function i(d) {
      return d === null ? "null" : typeof d == "string" ? `"${d}"` : String(d);
    }
    function o(d) {
      return d === null ? "null" : typeof d == "string" ? "str" : typeof d == "number" ? "num" : typeof d == "boolean" ? "bool" : "other";
    }
    return (d, c) => {
      const p = ar("JsonViewer", !0);
      return g(), b("div", {
        class: h(d.$style.node),
        style: Se({ paddingLeft: e.depth ? "12px" : "0" })
      }, [
        n.value ? (g(), b(le, { key: 0 }, [
          $("div", {
            class: h(d.$style.row),
            onClick: c[0] || (c[0] = (u) => l.value = !l.value)
          }, [
            $("span", {
              class: h(d.$style.chev)
            }, S(l.value ? "▾" : "▸"), 3),
            e.name !== void 0 ? (g(), b("span", {
              key: 0,
              class: h(d.$style.key)
            }, S(e.name) + ":", 3)) : N("", !0),
            $("span", {
              class: h(d.$style.summary)
            }, S(a.value), 3)
          ], 2),
          l.value ? (g(), b("div", _p, [
            (g(!0), b(le, null, ye(r.value, ([u, _]) => (g(), ce(p, {
              key: u,
              value: _,
              name: u,
              depth: e.depth + 1
            }, null, 8, ["value", "name", "depth"]))), 128))
          ])) : N("", !0)
        ], 64)) : (g(), b("div", {
          key: 1,
          class: h(d.$style.row)
        }, [
          e.name !== void 0 ? (g(), b("span", {
            key: 0,
            class: h(d.$style.key)
          }, S(e.name) + ":", 3)) : N("", !0),
          $("span", {
            class: h([d.$style.val, d.$style[o(e.value)]])
          }, S(i(e.value)), 3)
        ], 2))
      ], 6);
    };
  }
}), $p = "_node_bxy3l_2", vp = "_row_bxy3l_3", Ep = "_chev_bxy3l_4", Sp = "_summary_bxy3l_5", Tp = "_key_bxy3l_6", xp = "_val_bxy3l_7", Mp = "_str_bxy3l_8", Cp = "_num_bxy3l_9", Ap = "_bool_bxy3l_10", Ip = "_other_bxy3l_12", Op = {
  node: $p,
  row: vp,
  chev: Ep,
  summary: Sp,
  key: Tp,
  val: xp,
  str: Mp,
  num: Cp,
  bool: Ap,
  null: "_null_bxy3l_11",
  other: Ip
}, Np = {
  $style: Op
}, Tm = /* @__PURE__ */ P(wp, [["__cssModules", Np]]), Rp = /* @__PURE__ */ z({
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
      fe(S(e.label) + " ", 1),
      e.removable ? (g(), b("button", {
        key: 0,
        type: "button",
        class: h(s.$style.x),
        "aria-label": "Remove",
        onClick: r[0] || (r[0] = (a) => n("remove"))
      }, "×", 2)) : N("", !0)
    ], 2));
  }
}), Lp = "_tag_jpp5m_2", Bp = "_soft_jpp5m_3", zp = "_solid_jpp5m_4", Pp = "_x_jpp5m_5", Dp = {
  tag: Lp,
  soft: Bp,
  solid: zp,
  x: Pp
}, Up = {
  $style: Dp
}, qp = /* @__PURE__ */ P(Rp, [["__cssModules", Up]]), jp = ["placeholder"], Hp = /* @__PURE__ */ z({
  __name: "TagInput",
  props: {
    modelValue: {},
    placeholder: { default: "+ tag" },
    lowercase: { type: Boolean, default: !1 }
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const n = e, s = t, r = re("");
    function a() {
      let i = r.value.trim();
      n.lowercase && (i = i.toLowerCase()), i && !n.modelValue.includes(i) && s("update:modelValue", [...n.modelValue, i]), r.value = "";
    }
    function l(i) {
      s("update:modelValue", n.modelValue.filter((o) => o !== i));
    }
    return (i, o) => (g(), b("div", {
      class: h(i.$style.wrap)
    }, [
      (g(!0), b(le, null, ye(e.modelValue, (d) => (g(), ce(qp, {
        key: d,
        label: d,
        removable: "",
        onRemove: (c) => l(d)
      }, null, 8, ["label", "onRemove"]))), 128)),
      or($("input", {
        "onUpdate:modelValue": o[0] || (o[0] = (d) => r.value = d),
        class: h(i.$style.input),
        placeholder: e.placeholder,
        onKeyup: ir(a, ["enter"]),
        onBlur: a
      }, null, 42, jp), [
        [lr, r.value]
      ])
    ], 2));
  }
}), Vp = "_wrap_o9yrm_2", Fp = "_input_o9yrm_3", Gp = {
  wrap: Vp,
  input: Fp
}, Wp = {
  $style: Gp
}, xm = /* @__PURE__ */ P(Hp, [["__cssModules", Wp]]), Kp = /* @__PURE__ */ z({
  __name: "KeyValueList",
  props: {
    items: { default: () => [] }
  },
  setup(e) {
    return (t, n) => (g(), b("dl", {
      class: h(t.$style.list)
    }, [
      (g(!0), b(le, null, ye(e.items, (s, r) => (g(), b(le, { key: r }, [
        $("dt", {
          class: h(t.$style.k)
        }, S(s.label), 3),
        $("dd", {
          class: h(t.$style.v)
        }, S(s.value ?? "—"), 3)
      ], 64))), 128)),
      Q(t.$slots, "default")
    ], 2));
  }
}), Zp = "_list_162yo_2", Xp = "_k_162yo_3", Yp = "_v_162yo_4", Jp = {
  list: Zp,
  k: Xp,
  v: Yp
}, Qp = {
  $style: Jp
}, Mm = /* @__PURE__ */ P(Kp, [["__cssModules", Qp]]), ef = /* @__PURE__ */ z({
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
}), tf = "_spinner_1v1r5_2", nf = "_spin_1v1r5_2", sf = {
  spinner: tf,
  spin: nf
}, rf = {
  $style: sf
}, af = /* @__PURE__ */ P(ef, [["__cssModules", rf]]), of = /* @__PURE__ */ z({
  __name: "LoadingState",
  props: {
    label: { default: "Loading…" }
  },
  setup(e) {
    return (t, n) => (g(), b("div", {
      class: h(t.$style.wrap)
    }, [
      Ae(af, { size: 22 }),
      $("span", {
        class: h(t.$style.label)
      }, S(e.label), 3)
    ], 2));
  }
}), lf = "_wrap_101xf_2", cf = "_label_101xf_3", df = {
  wrap: lf,
  label: cf
}, uf = {
  $style: df
}, Cm = /* @__PURE__ */ P(of, [["__cssModules", uf]]), hf = ["aria-selected", "onClick"], pf = /* @__PURE__ */ z({
  __name: "Tabs",
  props: {
    modelValue: {},
    tabs: {}
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const n = t;
    return (s, r) => (g(), b("div", null, [
      $("div", {
        class: h(s.$style.bar),
        role: "tablist"
      }, [
        (g(!0), b(le, null, ye(e.tabs, (a) => (g(), b("button", {
          key: a.value,
          type: "button",
          role: "tab",
          "aria-selected": e.modelValue === a.value,
          class: h([s.$style.tab, e.modelValue === a.value && s.$style.active]),
          onClick: (l) => n("update:modelValue", a.value)
        }, [
          fe(S(a.label), 1),
          a.badge !== void 0 ? (g(), b("span", {
            key: 0,
            class: h(s.$style.badge)
          }, S(a.badge), 3)) : N("", !0)
        ], 10, hf))), 128))
      ], 2),
      Q(s.$slots, "default", { active: e.modelValue })
    ]));
  }
}), ff = "_bar_3rgvk_2", gf = "_tab_3rgvk_3", yf = "_active_3rgvk_5", bf = "_badge_3rgvk_6", mf = {
  bar: ff,
  tab: gf,
  active: yf,
  badge: bf
}, kf = {
  $style: mf
}, Am = /* @__PURE__ */ P(pf, [["__cssModules", kf]]), _f = /* @__PURE__ */ z({
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
      $("span", {
        class: h([t.$style.bubble, t.$style[e.placement]]),
        role: "tooltip"
      }, S(e.text), 3)
    ], 2));
  }
}), wf = "_wrap_1y5fd_2", $f = "_bubble_1y5fd_3", vf = "_top_1y5fd_4", Ef = "_bottom_1y5fd_5", Sf = {
  wrap: wf,
  bubble: $f,
  top: vf,
  bottom: Ef
}, Tf = {
  $style: Sf
}, Im = /* @__PURE__ */ P(_f, [["__cssModules", Tf]]), xf = /* @__PURE__ */ z({
  __name: "Collapsible",
  props: {
    open: { type: Boolean },
    title: {},
    defaultOpen: { type: Boolean, default: !1 }
  },
  emits: ["update:open"],
  setup(e, { emit: t }) {
    const n = e, s = t, r = re(n.open ?? n.defaultOpen);
    dt(() => n.open, (l) => {
      l !== void 0 && (r.value = l);
    });
    function a() {
      r.value = !r.value, s("update:open", r.value);
    }
    return (l, i) => (g(), b("div", {
      class: h(l.$style.wrap)
    }, [
      $("button", {
        type: "button",
        class: h(l.$style.head),
        onClick: a
      }, [
        $("span", {
          class: h(l.$style.chev)
        }, S(r.value ? "▾" : "▸"), 3),
        Q(l.$slots, "header", {}, () => [
          $("span", {
            class: h(l.$style.title)
          }, S(e.title), 3)
        ])
      ], 2),
      r.value ? (g(), b("div", {
        key: 0,
        class: h(l.$style.body)
      }, [
        Q(l.$slots, "default")
      ], 2)) : N("", !0)
    ], 2));
  }
}), Mf = "_wrap_a99st_2", Cf = "_head_a99st_3", Af = "_chev_a99st_5", If = "_title_a99st_6", Of = "_body_a99st_7", Nf = {
  wrap: Mf,
  head: Cf,
  chev: Af,
  title: If,
  body: Of
}, Rf = {
  $style: Nf
}, Om = /* @__PURE__ */ P(xf, [["__cssModules", Rf]]), Lf = /* @__PURE__ */ z({
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
    })[n.node.nodeType] ?? n.node.nodeType), a = ["memory_read", "memory_write", "if_else"], l = Y(() => a.includes(n.node.nodeType)), i = Y(() => {
      if (n.node.nodeType !== "ai" && n.node.nodeType !== "agent") return null;
      const u = n.node.config, _ = u.model ?? "", f = _.indexOf("/");
      return {
        provider: f > 0 ? _.slice(0, f) : null,
        model: f > 0 ? _.slice(f + 1) : _ || null,
        systemPrompt: u.system_prompt ?? null,
        toolCount: (u.tools ?? []).length
      };
    }), o = Y(() => {
      var u;
      return n.node.description ? d(n.node.description, 180) : (u = i.value) != null && u.systemPrompt ? d(i.value.systemPrompt, 140) : null;
    });
    function d(u, _) {
      return u.length <= _ ? u : u.slice(0, _).trimEnd() + "…";
    }
    function c(u) {
      !n.draggable || u.target.closest("[data-no-drag]") || s("pointer-down", u, n.node);
    }
    function p(u) {
      u.stopPropagation(), s("inspect", n.node);
    }
    return (u, _) => (g(), b("div", {
      class: h([
        u.$style.node,
        e.selected && u.$style.selected,
        e.orphan && u.$style.orphan,
        e.draggable && u.$style.draggable,
        e.dragging && u.$style.dragging
      ]),
      onPointerdown: c
    }, [
      Ae(_o, null, {
        head: be(() => [
          Ae(Ao, { variant: "live" }),
          $("span", {
            class: h(u.$style.name)
          }, S(e.node.label), 3),
          $("span", {
            class: h(u.$style.spacer)
          }, null, 2),
          l.value ? (g(), ce(vn, {
            key: 0,
            variant: "todo"
          }, {
            default: be(() => [..._[0] || (_[0] = [
              fe("deprecated", -1)
            ])]),
            _: 1
          })) : N("", !0),
          Ae(Pn, { variant: "inactive" }, {
            default: be(() => [
              fe(S(r.value), 1)
            ]),
            _: 1
          }),
          $("button", {
            type: "button",
            "data-no-drag": "",
            class: h(u.$style.inspectBtn),
            "aria-label": "Edit node",
            title: "Edit node",
            onClick: p
          }, "✎", 2)
        ]),
        foot: be(() => {
          var f, k, y;
          return [
            $("div", {
              class: h(u.$style.statGrid)
            }, [
              $("div", {
                class: h(u.$style.stat)
              }, [
                $("span", {
                  class: h(u.$style.statLabel)
                }, "PROVIDER", 2),
                $("span", {
                  class: h(u.$style.statValue)
                }, S(((f = i.value) == null ? void 0 : f.provider) ?? "—"), 3)
              ], 2),
              $("div", {
                class: h(u.$style.stat)
              }, [
                $("span", {
                  class: h(u.$style.statLabel)
                }, "MODEL", 2),
                $("span", {
                  class: h(u.$style.statValue)
                }, S(((k = i.value) == null ? void 0 : k.model) ?? "—"), 3)
              ], 2),
              $("div", {
                class: h(u.$style.stat)
              }, [
                $("span", {
                  class: h(u.$style.statLabel)
                }, "TOOLS", 2),
                $("span", {
                  class: h(u.$style.statValue)
                }, S(((y = i.value) == null ? void 0 : y.toolCount) ?? 0), 3)
              ], 2),
              $("div", {
                class: h([u.$style.stat, u.$style.statSplit])
              }, [
                $("div", {
                  class: h(u.$style.splitHalf)
                }, [
                  $("span", {
                    class: h(u.$style.statLabel)
                  }, "PORTS", 2),
                  $("span", {
                    class: h(u.$style.statValue)
                  }, S(e.inCount), 3)
                ], 2),
                $("div", {
                  class: h(u.$style.splitHalf)
                }, [
                  $("span", {
                    class: h(u.$style.statLabel)
                  }, "EXITS", 2),
                  $("span", {
                    class: h(u.$style.statValue)
                  }, S(e.outCount), 3)
                ], 2)
              ], 2)
            ], 2)
          ];
        }),
        default: be(() => [
          o.value ? (g(), b("p", {
            key: 0,
            class: h(u.$style.description)
          }, S(o.value), 3)) : (g(), b("p", {
            key: 1,
            class: h(u.$style.descriptionMissing)
          }, [
            Ae(vn, { variant: "todo" }, {
              default: be(() => [..._[1] || (_[1] = [
                fe("todo", -1)
              ])]),
              _: 1
            }),
            $("span", {
              class: h(u.$style.commentPrefix)
            }, "// description", 2)
          ], 2))
        ]),
        _: 1
      })
    ], 34));
  }
}), Bf = "_node_1ke6m_2", zf = "_draggable_1ke6m_12", Pf = "_dragging_1ke6m_15", Df = "_selected_1ke6m_19", Uf = "_orphan_1ke6m_23", qf = "_name_1ke6m_32", jf = "_spacer_1ke6m_43", Hf = "_inspectBtn_1ke6m_45", Vf = "_description_1ke6m_64", Ff = "_descriptionMissing_1ke6m_74", Gf = "_commentPrefix_1ke6m_76", Wf = "_statGrid_1ke6m_82", Kf = "_stat_1ke6m_82", Zf = "_statLabel_1ke6m_101", Xf = "_statValue_1ke6m_109", Yf = "_statSplit_1ke6m_118", Jf = "_splitHalf_1ke6m_120", Qf = {
  node: Bf,
  draggable: zf,
  dragging: Pf,
  selected: Df,
  orphan: Uf,
  name: qf,
  spacer: jf,
  inspectBtn: Hf,
  description: Vf,
  descriptionMissing: Ff,
  commentPrefix: Gf,
  statGrid: Wf,
  stat: Kf,
  statLabel: Zf,
  statValue: Xf,
  statSplit: Yf,
  splitHalf: Jf
}, eg = {
  $style: Qf
}, tg = /* @__PURE__ */ P(Lf, [["__cssModules", eg]]), ng = /* @__PURE__ */ z({
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
      }, S(e.label), 3)) : N("", !0),
      $("div", {
        class: h(t.$style.line)
      }, null, 2),
      $("span", {
        class: h(t.$style.arrow)
      }, null, 2)
    ], 2));
  }
}), sg = "_connector_1ewyl_2", rg = "_labeled_1ewyl_13", ag = "_line_1ewyl_18", og = "_arrow_1ewyl_28", ig = "_label_1ewyl_13", lg = {
  connector: sg,
  labeled: rg,
  line: ag,
  arrow: og,
  label: ig
}, cg = {
  $style: lg
}, Tn = /* @__PURE__ */ P(ng, [["__cssModules", cg]]);
function dg(e) {
  const t = new Map(e.nodes.map((o) => [o.id, o])), n = /* @__PURE__ */ new Map();
  for (const o of e.controlEdges) {
    const d = n.get(o.fromNodeId);
    d ? d.push(o) : n.set(o.fromNodeId, [o]);
  }
  const s = new Set(e.controlEdges.map((o) => o.toNodeId)), r = e.nodes.find((o) => !s.has(o.id)), a = [], l = /* @__PURE__ */ new Set();
  let i = r;
  for (; i && !l.has(i.id); ) {
    l.add(i.id);
    const o = n.get(i.id) ?? [];
    if (a.push({
      kind: "node",
      key: i.id,
      node: i,
      branchCount: o.length > 1 ? o.length - 1 : 0
    }), o.length === 0) break;
    const d = o[0];
    a.push({ kind: "edge", key: d.id, port: d.fromPort }), i = t.get(d.toNodeId);
  }
  for (const o of e.nodes)
    l.has(o.id) || a.push({ kind: "node", key: o.id, node: o, orphan: !0 });
  return a;
}
const Nt = 5, Le = re(null), Ke = re(null);
let Xt = null, Yt = null;
function Ss(e) {
  Le.value && (Le.value = {
    ...Le.value,
    cursorX: e.clientX,
    cursorY: e.clientY
  });
}
function Ts() {
  const e = Xt, t = Ke.value;
  Ms(), e == null || e(t);
}
function xs(e) {
  if (e.key === "Escape") {
    const t = Yt;
    Ms(), t == null || t();
  }
}
function Ms() {
  window.removeEventListener("pointermove", Ss), window.removeEventListener("pointerup", Ts), window.removeEventListener("keydown", xs), Le.value = null, Ke.value = null, Xt = null, Yt = null;
}
function ug() {
  return {
    drag: Le,
    isDragging: Y(() => Le.value !== null),
    dropIndex: Ke,
    beginDrag(e, t) {
      const n = t.sourceRect;
      Le.value = {
        source: e,
        cursorX: t.cursorX,
        cursorY: t.cursorY,
        offsetX: n ? t.cursorX - n.left : 0,
        offsetY: n ? t.cursorY - n.top : 0
      }, Ke.value = null, Xt = t.onDrop ?? null, Yt = t.onCancel ?? null, window.addEventListener("pointermove", Ss, { passive: !0 }), window.addEventListener("pointerup", Ts, { passive: !0 }), window.addEventListener("keydown", xs);
    },
    setDropIndex(e) {
      Ke.value = e;
    },
    THRESHOLD_PX: Nt
  };
}
function Cs(e, t, n, s) {
  const r = e - n, a = t - s;
  return r * r + a * a >= Nt * Nt;
}
const hg = {
  key: 3,
  style: { "font-family": "var(--font-mono)", "font-size": "13px", color: "var(--color-muted)", "align-self": "center", padding: "0 16px", "white-space": "nowrap" }
}, $t = "__phantom__", pg = /* @__PURE__ */ z({
  __name: "FlowCanvas",
  props: {
    flow: {},
    selectedNodeId: {},
    reorderable: { type: Boolean, default: !1 },
    droppable: { type: Boolean, default: !1 }
  },
  emits: ["select", "drop"],
  setup(e, { expose: t, emit: n }) {
    const s = e, r = n, a = Y(() => dg(s.flow)), l = Y(
      () => a.value.filter((E) => E.kind === "node").map((E) => E.kind === "node" ? E.node.id : "")
    ), i = Y(() => new Map(s.flow.nodes.map((E) => [E.id, E]))), o = Y(() => {
      const E = /* @__PURE__ */ new Map(), I = /* @__PURE__ */ new Map();
      for (const O of s.flow.controlEdges)
        E.set(O.toNodeId, (E.get(O.toNodeId) ?? 0) + 1), I.set(O.fromNodeId, (I.get(O.fromNodeId) ?? 0) + 1);
      return { inCount: E, outCount: I };
    });
    Y(() => s.reorderable || s.droppable);
    const d = ug(), c = re(null), p = re(null);
    let u = null;
    const _ = re(null), f = Y(() => {
      const E = l.value, I = p.value, O = d.dropIndex.value;
      if (!I || O === null) return E;
      if (I.kind === "node") {
        const m = E.indexOf(I.nodeId);
        if (m === -1) return E;
        const w = E.filter((v) => v !== I.nodeId), T = m < O ? O - 1 : O;
        return [...w.slice(0, T), I.nodeId, ...w.slice(T)];
      }
      return I.kind === "palette" ? [...E.slice(0, O), $t, ...E.slice(O)] : E;
    }), k = Y(() => {
      var m;
      const E = f.value;
      if (E.length === 0) return [];
      const I = [];
      for (let w = 0; w < E.length; w++) {
        const T = E[w];
        if (T === $t) {
          const v = p.value, U = (v == null ? void 0 : v.kind) === "palette" ? v.item : { id: "x", nodeType: "unknown", label: "New node" };
          I.push({ kind: "phantom", key: $t, phantom: U });
        } else {
          const v = i.value.get(T);
          if (!v) continue;
          const U = ((m = p.value) == null ? void 0 : m.kind) === "node" && p.value.nodeId === T;
          I.push({
            kind: "node",
            key: T,
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
        w < E.length - 1 && I.push({ kind: "edge", key: `edge-${w}` });
      }
      const O = new Set(E);
      for (const w of s.flow.nodes)
        O.has(w.id) || (I.length > 0 && I.push({ kind: "edge", key: `edge-orphan-${w.id}` }), I.push({
          kind: "node",
          key: w.id,
          node: {
            id: w.id,
            nodeType: w.nodeType,
            label: w.label,
            description: w.description,
            config: w.config
          },
          orphan: !0
        }));
      return I;
    }), y = Y(() => s.flow.nodes.length === 0);
    function C(E, I) {
      if (!s.reorderable) return;
      const O = E.currentTarget ?? E.target.closest("[data-node-slot]");
      O && (u = {
        source: { kind: "node", nodeId: I.id },
        originX: E.clientX,
        originY: E.clientY,
        rect: O.getBoundingClientRect(),
        nodeEl: O
      }, window.addEventListener("pointermove", D), window.addEventListener("pointerup", L), window.addEventListener("pointercancel", L));
    }
    function D(E) {
      u && Cs(E.clientX, E.clientY, u.originX, u.originY) && (M(u.source, E.clientX, E.clientY, u.rect), B());
    }
    function L() {
      const E = u == null ? void 0 : u.source;
      if (B(), (E == null ? void 0 : E.kind) === "node") {
        const I = i.value.get(E.nodeId);
        I && r("select", I);
      }
    }
    function B() {
      u = null, window.removeEventListener("pointermove", D), window.removeEventListener("pointerup", L), window.removeEventListener("pointercancel", L);
    }
    function G(E, I, O) {
      M({ kind: "palette", item: E }, I.clientX, I.clientY, O);
    }
    function M(E, I, O, m) {
      p.value = E;
      const w = (m == null ? void 0 : m.width) ?? 280, T = (m == null ? void 0 : m.height) ?? 180, v = m ? I - m.left : w / 2, U = m ? O - m.top : T / 2;
      c.value = { x: I - v, y: O - U, w, h: T, payload: E }, d.beginDrag(E, {
        cursorX: I,
        cursorY: O,
        sourceRect: m,
        onDrop: te,
        onCancel: de
      }), window.addEventListener("pointermove", q, { passive: !0 });
    }
    function q(E) {
      var m, w;
      if (!c.value) return;
      const I = ((m = d.drag.value) == null ? void 0 : m.offsetX) ?? c.value.w / 2, O = ((w = d.drag.value) == null ? void 0 : w.offsetY) ?? c.value.h / 2;
      c.value = { ...c.value, x: E.clientX - I, y: E.clientY - O }, d.setDropIndex(x(E.clientX));
    }
    function x(E) {
      const I = _.value;
      if (!I) return null;
      const O = Array.from(I.querySelectorAll("[data-node-slot]"));
      if (O.length === 0) return 0;
      let m = 0;
      for (const w of O) {
        const T = w.getBoundingClientRect();
        if (E > T.left + T.width / 2) m++;
        else break;
      }
      return m;
    }
    function te(E) {
      const I = p.value;
      if (I && E !== null) {
        const O = ae(E, I);
        O && r("drop", { source: I, target: O });
      }
      se();
    }
    function ae(E, I) {
      const O = l.value, m = I.kind === "node" ? O.filter((v) => v !== I.nodeId) : O;
      if (I.kind === "node") {
        const v = O.indexOf(I.nodeId);
        if ((v < E ? E - 1 : E) === v) return null;
      }
      const w = m[E] ?? null;
      return { kind: "between-nodes", afterNodeId: m[E - 1] ?? null, beforeNodeId: w };
    }
    function de() {
      se();
    }
    function se() {
      p.value = null, c.value = null, window.removeEventListener("pointermove", q);
    }
    return Lt(() => {
      B(), se();
    }), t({ startPaletteDrag: G }), (E, I) => {
      var O, m;
      return g(), b("div", {
        class: h(E.$style.scroll)
      }, [
        $("div", {
          class: h(E.$style.inner),
          ref_key: "slotsEl",
          ref: _
        }, [
          E.$slots.lead ? (g(), b("div", {
            key: 0,
            class: h(E.$style.slot)
          }, [
            Q(E.$slots, "lead")
          ], 2)) : N("", !0),
          E.$slots.lead && !y.value ? (g(), ce(Tn, { key: 1 })) : N("", !0),
          y.value && !E.$slots.lead ? (g(), ce(vi, {
            key: 2,
            title: "Empty flow",
            sub: "Drag a node from the palette to get started."
          })) : y.value ? (g(), b("span", hg, "// add an Agent node from the palette →")) : (g(!0), b(le, { key: 4 }, ye(k.value, (w) => {
            var T, v;
            return g(), b(le, {
              key: w.key
            }, [
              w.kind === "node" ? (g(), b("div", {
                key: 0,
                "data-node-slot": "",
                class: h(E.$style.slot)
              }, [
                Ae(tg, {
                  node: w.node,
                  selected: w.node.id === e.selectedNodeId,
                  orphan: w.orphan,
                  dragging: w.isDragging,
                  draggable: e.reorderable && !w.orphan,
                  "in-count": o.value.inCount.get(w.node.id) ?? 0,
                  "out-count": o.value.outCount.get(w.node.id) ?? 0,
                  onPointerDown: C,
                  onInspect: I[0] || (I[0] = (U) => r("select", U))
                }, null, 8, ["node", "selected", "orphan", "dragging", "draggable", "in-count", "out-count"])
              ], 2)) : w.kind === "phantom" ? (g(), b("div", {
                key: 1,
                "data-node-slot": "",
                class: h([E.$style.slot, E.$style.phantom])
              }, [
                $("div", {
                  class: h(E.$style.phantomCard)
                }, [
                  $("span", {
                    class: h(E.$style.phantomLabel)
                  }, S(((T = w.phantom) == null ? void 0 : T.label) ?? "New node"), 3),
                  $("span", {
                    class: h(E.$style.phantomType)
                  }, S(((v = w.phantom) == null ? void 0 : v.nodeType) ?? ""), 3)
                ], 2)
              ], 2)) : (g(), ce(Tn, { key: 2 }))
            ], 64);
          }), 128))
        ], 2),
        (g(), ce(ut, { to: "body" }, [
          c.value ? (g(), b("div", {
            key: 0,
            class: h(E.$style.ghost),
            style: Se({
              transform: `translate(${c.value.x}px, ${c.value.y}px)`,
              width: c.value.w + "px"
            })
          }, [
            $("div", {
              class: h(E.$style.ghostInner)
            }, [
              c.value.payload.kind === "node" ? (g(), b(le, { key: 0 }, [
                $("span", {
                  class: h(E.$style.ghostLabel)
                }, S(((O = i.value.get(c.value.payload.nodeId)) == null ? void 0 : O.label) ?? "Node"), 3),
                $("span", {
                  class: h(E.$style.ghostType)
                }, S(((m = i.value.get(c.value.payload.nodeId)) == null ? void 0 : m.nodeType) ?? ""), 3)
              ], 64)) : c.value.payload.kind === "palette" ? (g(), b(le, { key: 1 }, [
                $("span", {
                  class: h(E.$style.ghostLabel)
                }, S(c.value.payload.item.label), 3),
                $("span", {
                  class: h(E.$style.ghostType)
                }, S(c.value.payload.item.nodeType), 3)
              ], 64)) : N("", !0)
            ], 2)
          ], 6)) : N("", !0)
        ]))
      ], 2);
    };
  }
}), fg = "_scroll_1luup_2", gg = "_inner_1luup_21", yg = "_slot_1luup_33", bg = "_phantom_1luup_47", mg = "_phantomCard_1luup_52", kg = "_phantomLabel_1luup_65", _g = "_phantomType_1luup_70", wg = "_ghost_1luup_79", $g = "_ghostInner_1luup_90", vg = "_ghostLabel_1luup_101", Eg = "_ghostType_1luup_107", Sg = {
  scroll: fg,
  inner: gg,
  slot: yg,
  phantom: bg,
  phantomCard: mg,
  phantomLabel: kg,
  phantomType: _g,
  ghost: wg,
  ghostInner: $g,
  ghostLabel: vg,
  ghostType: Eg
}, Tg = {
  $style: Sg
}, Nm = /* @__PURE__ */ P(pg, [["__cssModules", Tg]]), xg = ["onPointerdown"], Mg = /* @__PURE__ */ z({
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
    function r(o, d) {
      s = { item: d, originX: o.clientX, originY: o.clientY }, window.addEventListener("pointermove", a), window.addEventListener("pointerup", l), window.addEventListener("pointercancel", l);
    }
    function a(o) {
      s && Cs(o.clientX, o.clientY, s.originX, s.originY) && (n("start-drag", s.item, o), i());
    }
    function l() {
      i();
    }
    function i() {
      s = null, window.removeEventListener("pointermove", a), window.removeEventListener("pointerup", l), window.removeEventListener("pointercancel", l);
    }
    return (o, d) => (g(), ce(cr, {
      name: o.$style.slide
    }, {
      default: be(() => [
        e.open ? (g(), b("aside", {
          key: 0,
          class: h(o.$style.panel),
          style: Se({ width: e.width + "px" }),
          role: "dialog",
          "aria-label": "Add node"
        }, [
          $("header", {
            class: h(o.$style.head)
          }, [
            $("span", {
              class: h(o.$style.title)
            }, S(e.title), 3),
            $("button", {
              type: "button",
              class: h(o.$style.closeBtn),
              "aria-label": "Close palette",
              onClick: d[0] || (d[0] = (c) => n("close"))
            }, "×", 2)
          ], 2),
          $("div", {
            class: h(o.$style.body)
          }, [
            $("p", {
              class: h(o.$style.hint)
            }, [
              $("span", {
                class: h(o.$style.comment)
              }, "// drag onto the canvas", 2)
            ], 2),
            $("ul", {
              class: h(o.$style.list)
            }, [
              (g(!0), b(le, null, ye(e.items, (c) => (g(), b("li", {
                key: c.id,
                class: h(o.$style.item),
                onPointerdown: (p) => r(p, c)
              }, [
                $("div", {
                  class: h(o.$style.itemHead)
                }, [
                  $("span", {
                    class: h(o.$style.itemLabel)
                  }, S(c.label), 3),
                  $("span", {
                    class: h(o.$style.itemType)
                  }, S(c.nodeType), 3)
                ], 2),
                c.description ? (g(), b("p", {
                  key: 0,
                  class: h(o.$style.itemDesc)
                }, S(c.description), 3)) : N("", !0)
              ], 42, xg))), 128))
            ], 2)
          ], 2)
        ], 6)) : N("", !0)
      ]),
      _: 1
    }, 8, ["name"]));
  }
}), Cg = "_panel_djxcr_2", Ag = "_head_djxcr_16", Ig = "_title_djxcr_25", Og = "_closeBtn_djxcr_27", Ng = "_body_djxcr_39", Rg = "_hint_djxcr_48", Lg = "_comment_djxcr_49", Bg = "_list_djxcr_55", zg = "_item_djxcr_64", Pg = "_itemHead_djxcr_84", Dg = "_itemLabel_djxcr_91", Ug = "_itemType_djxcr_93", qg = "_itemDesc_djxcr_101", jg = {
  panel: Cg,
  head: Ag,
  title: Ig,
  closeBtn: Og,
  body: Ng,
  hint: Rg,
  comment: Lg,
  list: Bg,
  item: zg,
  itemHead: Pg,
  itemLabel: Dg,
  itemType: Ug,
  itemDesc: qg,
  "slide-enter-from": "_slide-enter-from_djxcr_108",
  "slide-leave-to": "_slide-leave-to_djxcr_108",
  "slide-enter-active": "_slide-enter-active_djxcr_109",
  "slide-leave-active": "_slide-leave-active_djxcr_109",
  "slide-enter-to": "_slide-enter-to_djxcr_110",
  "slide-leave-from": "_slide-leave-from_djxcr_110"
}, Hg = {
  $style: jg
}, Rm = /* @__PURE__ */ P(Mg, [["__cssModules", Hg]]), Fe = { accentHue: 287, accentL: 0.66, accentC: 0.17, baseHue: 285, baseChroma: 1 };
function vt(e) {
  return e /= 255, e <= 0.04045 ? e / 12.92 : Math.pow((e + 0.055) / 1.055, 2.4);
}
function Vg(e, t, n) {
  const s = vt(e), r = vt(t), a = vt(n), l = 0.4122214708 * s + 0.5363325363 * r + 0.0514459929 * a, i = 0.2119034982 * s + 0.6806995451 * r + 0.1073969566 * a, o = 0.0883024619 * s + 0.2817188376 * r + 0.6299787005 * a, d = Math.cbrt(l), c = Math.cbrt(i), p = Math.cbrt(o), u = 0.2104542553 * d + 0.793617785 * c - 0.0040720468 * p, _ = 1.9779984951 * d - 2.428592205 * c + 0.4505937099 * p, f = 0.0259040371 * d + 0.7827717662 * c - 0.808675766 * p, k = Math.sqrt(_ * _ + f * f);
  let y = Math.atan2(f, _) * 180 / Math.PI;
  return y < 0 && (y += 360), { L: u, C: k, H: y };
}
const Ge = (e, t = 3) => Math.round(e * 10 ** t) / 10 ** t, Et = (e, t, n) => Math.min(n, Math.max(t, e));
function Fg(e, t = 48) {
  const n = document.createElement("canvas");
  n.width = t, n.height = t;
  const s = n.getContext("2d", { willReadFrequently: !0 });
  if (!s) return { ...Fe };
  try {
    s.drawImage(e, 0, 0, t, t);
  } catch {
    return { ...Fe };
  }
  let r;
  try {
    r = s.getImageData(0, 0, t, t).data;
  } catch {
    return { ...Fe };
  }
  let a = { C: -1, L: 0.66, H: 287 }, l = 0, i = 0, o = 0, d = 0;
  for (let u = 0; u < r.length; u += 4) {
    if (r[u + 3] < 128) continue;
    const { L: _, C: f, H: k } = Vg(r[u], r[u + 1], r[u + 2]);
    if (f > a.C && _ > 0.45 && _ < 0.85 && (a = { C: f, L: _, H: k }), f > 0.02) {
      const y = k * Math.PI / 180;
      l += Math.cos(y) * f, i += Math.sin(y) * f, o += f, d++;
    }
  }
  const c = d ? (Math.atan2(i, l) * 180 / Math.PI + 360) % 360 : Fe.baseHue, p = d ? o / d : 0.05;
  return {
    accentHue: Ge(a.C >= 0 ? a.H : Fe.accentHue, 1),
    accentL: Ge(Et(a.L, 0.58, 0.78)),
    // keep it readable as an accent
    accentC: Ge(Et(a.C, 0.1, 0.2)),
    // vivid but not neon
    baseHue: Ge(c, 1),
    baseChroma: Ge(Et(p * 12, 0.4, 1.8), 2)
    // saturated image → stronger surface tint
  };
}
const Rt = {
  accentHue: "--accent-hue",
  accentL: "--accent-l",
  accentC: "--accent-c",
  baseHue: "--base-hue",
  baseChroma: "--base-chroma"
};
function Gg(e, t = document.documentElement) {
  Object.keys(Rt).forEach((n) => t.style.setProperty(Rt[n], String(e[n])));
}
function Lm(e = document.documentElement) {
  Object.values(Rt).forEach((t) => e.style.removeProperty(t));
}
function Wg(e) {
  return new Promise((t, n) => {
    const s = new Image();
    s.crossOrigin = "anonymous", s.onload = () => t(s), s.onerror = () => n(new Error(`failed to load image: ${e}`)), s.src = e;
  });
}
async function Bm(e, t) {
  const n = await Wg(e), s = Fg(n);
  return Gg(s, t), s;
}
async function X(e) {
  const { data: t, error: n, response: s } = await e;
  if (n !== void 0 || !(s != null && s.ok)) {
    const r = (n == null ? void 0 : n.message) ?? (s ? `${s.status} ${s.statusText}` : "Network error");
    throw new Error(r);
  }
  return t;
}
function Jt(e, t) {
  const n = {};
  for (const [s, r] of Object.entries(e))
    n[s] = typeof r == "function" ? (a) => r({ ...a, client: t }) : r;
  return n;
}
const Kg = {
  bodySerializer: (e) => JSON.stringify(e, (t, n) => typeof n == "bigint" ? n.toString() : n)
};
function Zg({
  onRequest: e,
  onSseError: t,
  onSseEvent: n,
  responseTransformer: s,
  responseValidator: r,
  sseDefaultRetryDelay: a,
  sseMaxRetryAttempts: l,
  sseMaxRetryDelay: i,
  sseSleepFn: o,
  url: d,
  ...c
}) {
  let p;
  const u = o ?? ((k) => new Promise((y) => setTimeout(y, k)));
  return { stream: async function* () {
    let k = a ?? 3e3, y = 0;
    const C = c.signal ?? new AbortController().signal;
    for (; !C.aborted; ) {
      y++;
      const D = c.headers instanceof Headers ? c.headers : new Headers(c.headers);
      p !== void 0 && D.set("Last-Event-ID", p);
      try {
        const L = {
          redirect: "follow",
          ...c,
          body: c.serializedBody,
          headers: D,
          signal: C
        };
        let B = new Request(d, L);
        e && (B = await e(d, L));
        const M = await (c.fetch ?? globalThis.fetch)(B);
        if (!M.ok) throw new Error(`SSE failed: ${M.status} ${M.statusText}`);
        if (!M.body) throw new Error("No body in SSE response");
        const q = M.body.pipeThrough(new TextDecoderStream()).getReader();
        let x = "";
        const te = () => {
          try {
            q.cancel();
          } catch {
          }
        };
        C.addEventListener("abort", te);
        try {
          for (; ; ) {
            const { done: ae, value: de } = await q.read();
            if (ae) break;
            x += de, x = x.replace(/\r\n?/g, `
`);
            const se = x.split(`

`);
            x = se.pop() ?? "";
            for (const E of se) {
              const I = E.split(`
`), O = [];
              let m;
              for (const v of I)
                if (v.startsWith("data:"))
                  O.push(v.replace(/^data:\s*/, ""));
                else if (v.startsWith("event:"))
                  m = v.replace(/^event:\s*/, "");
                else if (v.startsWith("id:"))
                  p = v.replace(/^id:\s*/, "");
                else if (v.startsWith("retry:")) {
                  const U = Number.parseInt(v.replace(/^retry:\s*/, ""), 10);
                  Number.isNaN(U) || (k = U);
                }
              let w, T = !1;
              if (O.length) {
                const v = O.join(`
`);
                try {
                  w = JSON.parse(v), T = !0;
                } catch {
                  w = v;
                }
              }
              T && (r && await r(w), s && (w = await s(w))), n == null || n({
                data: w,
                event: m,
                id: p,
                retry: k
              }), O.length && (yield w);
            }
          }
        } finally {
          C.removeEventListener("abort", te), q.releaseLock();
        }
        break;
      } catch (L) {
        if (t == null || t(L), l !== void 0 && y >= l)
          break;
        const B = Math.min(k * 2 ** (y - 1), i ?? 3e4);
        await u(B);
      }
    }
  }() };
}
const Xg = (e) => {
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
}, Yg = (e) => {
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
}, Jg = (e) => {
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
}, As = ({
  allowReserved: e,
  explode: t,
  name: n,
  style: s,
  value: r
}) => {
  if (!t) {
    const i = (e ? r : r.map((o) => encodeURIComponent(o))).join(Yg(s));
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
  const a = Xg(s), l = r.map((i) => s === "label" || s === "simple" ? e ? i : encodeURIComponent(i) : gt({
    allowReserved: e,
    name: n,
    value: i
  })).join(a);
  return s === "label" || s === "matrix" ? a + l : l;
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
}, Is = ({
  allowReserved: e,
  explode: t,
  name: n,
  style: s,
  value: r,
  valueOnly: a
}) => {
  if (r instanceof Date)
    return a ? r.toISOString() : `${n}=${r.toISOString()}`;
  if (s !== "deepObject" && !t) {
    let o = [];
    Object.entries(r).forEach(([c, p]) => {
      o = [...o, c, e ? p : encodeURIComponent(p)];
    });
    const d = o.join(",");
    switch (s) {
      case "form":
        return `${n}=${d}`;
      case "label":
        return `.${d}`;
      case "matrix":
        return `;${n}=${d}`;
      default:
        return d;
    }
  }
  const l = Jg(s), i = Object.entries(r).map(
    ([o, d]) => gt({
      allowReserved: e,
      name: s === "deepObject" ? `${n}[${o}]` : o,
      value: d
    })
  ).join(l);
  return s === "label" || s === "matrix" ? l + i : i;
}, Qg = /\{[^{}]+\}/g, ey = ({ path: e, url: t }) => {
  let n = t;
  const s = t.match(Qg);
  if (s)
    for (const r of s) {
      let a = !1, l = r.substring(1, r.length - 1), i = "simple";
      l.endsWith("*") && (a = !0, l = l.substring(0, l.length - 1)), l.startsWith(".") ? (l = l.substring(1), i = "label") : l.startsWith(";") && (l = l.substring(1), i = "matrix");
      const o = e[l];
      if (o == null)
        continue;
      if (Array.isArray(o)) {
        n = n.replace(r, As({ explode: a, name: l, style: i, value: o }));
        continue;
      }
      if (typeof o == "object") {
        n = n.replace(
          r,
          Is({
            explode: a,
            name: l,
            style: i,
            value: o,
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
            value: o
          })}`
        );
        continue;
      }
      const d = encodeURIComponent(
        i === "label" ? `.${o}` : o
      );
      n = n.replace(r, d);
    }
  return n;
}, ty = ({
  baseUrl: e,
  path: t,
  query: n,
  querySerializer: s,
  url: r
}) => {
  const a = r.startsWith("/") ? r : `/${r}`;
  let l = (e ?? "") + a;
  t && (l = ey({ path: t, url: l }));
  let i = n ? s(n) : "";
  return i.startsWith("?") && (i = i.substring(1)), i && (l += `?${i}`), l;
};
function xn(e) {
  const t = e.body !== void 0;
  if (t && e.bodySerializer)
    return "serializedBody" in e ? e.serializedBody !== void 0 && e.serializedBody !== "" ? e.serializedBody : null : e.body !== "" ? e.body : null;
  if (t)
    return e.body;
}
const ny = async (e, t) => {
  const n = typeof t == "function" ? await t(e) : t;
  if (n)
    return e.scheme === "bearer" ? `Bearer ${n}` : e.scheme === "basic" ? `Basic ${btoa(n)}` : n;
}, Os = ({
  parameters: e = {},
  ...t
} = {}) => (s) => {
  const r = [];
  if (s && typeof s == "object")
    for (const a in s) {
      const l = s[a];
      if (l == null)
        continue;
      const i = e[a] || t;
      if (Array.isArray(l)) {
        const o = As({
          allowReserved: i.allowReserved,
          explode: !0,
          name: a,
          style: "form",
          value: l,
          ...i.array
        });
        o && r.push(o);
      } else if (typeof l == "object") {
        const o = Is({
          allowReserved: i.allowReserved,
          explode: !0,
          name: a,
          style: "deepObject",
          value: l,
          ...i.object
        });
        o && r.push(o);
      } else {
        const o = gt({
          allowReserved: i.allowReserved,
          name: a,
          value: l
        });
        o && r.push(o);
      }
    }
  return r.join("&");
}, sy = (e) => {
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
}, ry = (e, t) => {
  var n, s;
  return t ? !!(e.headers.has(t) || (n = e.query) != null && n[t] || (s = e.headers.get("Cookie")) != null && s.includes(`${t}=`)) : !1;
};
async function ay(e) {
  for (const t of e.security ?? []) {
    if (ry(e, t.name))
      continue;
    const n = await ny(t, e.auth);
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
const Mn = (e) => ty({
  baseUrl: e.baseUrl,
  path: e.path,
  query: e.query,
  querySerializer: typeof e.querySerializer == "function" ? e.querySerializer : Os(e.querySerializer),
  url: e.url
}), Cn = (e, t) => {
  var s;
  const n = { ...e, ...t };
  return (s = n.baseUrl) != null && s.endsWith("/") && (n.baseUrl = n.baseUrl.substring(0, n.baseUrl.length - 1)), n.headers = Ns(e.headers, t.headers), n;
}, oy = (e) => {
  const t = [];
  return e.forEach((n, s) => {
    t.push([s, n]);
  }), t;
}, Ns = (...e) => {
  const t = new Headers();
  for (const n of e) {
    if (!n)
      continue;
    const s = n instanceof Headers ? oy(n) : Object.entries(n);
    for (const [r, a] of s)
      if (a === null)
        t.delete(r);
      else if (Array.isArray(a))
        for (const l of a)
          t.append(r, l);
      else a !== void 0 && t.set(
        r,
        typeof a == "object" ? JSON.stringify(a) : a
      );
  }
  return t;
};
let St = class {
  constructor() {
    V(this, "fns", []);
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
const iy = () => ({
  error: new St(),
  request: new St(),
  response: new St()
}), ly = Os({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), cy = {
  "Content-Type": "application/json"
}, Qt = (e = {}) => ({
  ...Kg,
  headers: cy,
  parseAs: "auto",
  querySerializer: ly,
  ...e
}), Rs = (e = {}) => {
  let t = Cn(Qt(), e);
  const n = () => ({ ...t }), s = (c) => (t = Cn(t, c), n()), r = iy(), a = async (c) => {
    const p = {
      ...t,
      ...c,
      fetch: c.fetch ?? t.fetch ?? globalThis.fetch,
      headers: Ns(t.headers, c.headers),
      serializedBody: void 0
    };
    p.security && await ay(p), p.requestValidator && await p.requestValidator(p), p.body !== void 0 && p.bodySerializer && (p.serializedBody = p.bodySerializer(p.body)), (p.body === void 0 || p.serializedBody === "") && p.headers.delete("Content-Type");
    const u = p, _ = Mn(u);
    return { opts: u, url: _ };
  }, l = async (c) => {
    const p = c.throwOnError ?? t.throwOnError, u = c.responseStyle ?? t.responseStyle;
    let _, f;
    try {
      const { opts: k, url: y } = await a(c), C = {
        redirect: "follow",
        ...k,
        body: xn(k)
      };
      _ = new Request(y, C);
      for (const M of r.request.fns)
        M && (_ = await M(_, k));
      const D = k.fetch;
      f = await D(_);
      for (const M of r.response.fns)
        M && (f = await M(f, _, k));
      const L = {
        request: _,
        response: f
      };
      if (f.ok) {
        const M = (k.parseAs === "auto" ? sy(f.headers.get("Content-Type")) : k.parseAs) ?? "json";
        if (f.status === 204 || f.headers.get("Content-Length") === "0") {
          let x;
          switch (M) {
            case "arrayBuffer":
            case "blob":
            case "text":
              x = await f[M]();
              break;
            case "formData":
              x = new FormData();
              break;
            case "stream":
              x = f.body;
              break;
            case "json":
            default:
              x = {};
              break;
          }
          return k.responseStyle === "data" ? x : {
            data: x,
            ...L
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
            const x = await f.text();
            q = x ? JSON.parse(x) : {};
            break;
          }
          case "stream":
            return k.responseStyle === "data" ? f.body : {
              data: f.body,
              ...L
            };
        }
        return M === "json" && (k.responseValidator && await k.responseValidator(q), k.responseTransformer && (q = await k.responseTransformer(q))), k.responseStyle === "data" ? q : {
          data: q,
          ...L
        };
      }
      const B = await f.text();
      let G;
      try {
        G = JSON.parse(B);
      } catch {
      }
      throw G ?? B;
    } catch (k) {
      let y = k;
      for (const C of r.error.fns)
        C && (y = await C(y, f, _, c));
      if (y = y || {}, p)
        throw y;
      return u === "data" ? void 0 : {
        error: y,
        request: _,
        response: f
      };
    }
  }, i = (c) => (p) => l({ ...p, method: c }), o = (c) => async (p) => {
    const { opts: u, url: _ } = await a(p);
    return Zg({
      ...u,
      body: u.body,
      method: c,
      onRequest: async (f, k) => {
        let y = new Request(f, k);
        for (const C of r.request.fns)
          C && (y = await C(y, u));
        return y;
      },
      serializedBody: xn(u),
      url: _
    });
  };
  return {
    buildUrl: (c) => Mn({ ...t, ...c }),
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
      connect: o("CONNECT"),
      delete: o("DELETE"),
      get: o("GET"),
      head: o("HEAD"),
      options: o("OPTIONS"),
      patch: o("PATCH"),
      post: o("POST"),
      put: o("PUT"),
      trace: o("TRACE")
    },
    trace: i("TRACE")
  };
}, Te = Rs(Qt()), dy = (e) => ((e == null ? void 0 : e.client) ?? Te).get({ url: "/ping", ...e }), uy = (e) => ((e == null ? void 0 : e.client) ?? Te).get({ url: "/tools", ...e }), hy = (e) => ((e == null ? void 0 : e.client) ?? Te).get({ url: "/tools/grouped", ...e }), py = (e) => ((e == null ? void 0 : e.client) ?? Te).get({ url: "/tools/all", ...e }), fy = (e) => ((e == null ? void 0 : e.client) ?? Te).post({ url: "/tools/rebuild", ...e }), gy = (e) => (e.client ?? Te).post({ url: "/tools/{name}/enable", ...e }), yy = (e) => (e.client ?? Te).post({ url: "/tools/{name}/disable", ...e }), by = (e) => ((e == null ? void 0 : e.client) ?? Te).get({ url: "/toolboxes/", ...e }), my = (e) => (e.client ?? Te).post({
  url: "/toolboxes/mcp",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), ky = (e) => (e.client ?? Te).delete({ url: "/toolboxes/{id}", ...e }), _y = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  deleteToolboxesById: ky,
  getPing: dy,
  getToolboxes: by,
  getTools: uy,
  getToolsAll: py,
  getToolsGrouped: hy,
  postToolboxesMcp: my,
  postToolsByNameDisable: yy,
  postToolsByNameEnable: gy,
  postToolsRebuild: fy
}, Symbol.toStringTag, { value: "Module" })), wy = {
  bodySerializer: (e) => JSON.stringify(e, (t, n) => typeof n == "bigint" ? n.toString() : n)
};
function $y({
  onRequest: e,
  onSseError: t,
  onSseEvent: n,
  responseTransformer: s,
  responseValidator: r,
  sseDefaultRetryDelay: a,
  sseMaxRetryAttempts: l,
  sseMaxRetryDelay: i,
  sseSleepFn: o,
  url: d,
  ...c
}) {
  let p;
  const u = o ?? ((k) => new Promise((y) => setTimeout(y, k)));
  return { stream: async function* () {
    let k = a ?? 3e3, y = 0;
    const C = c.signal ?? new AbortController().signal;
    for (; !C.aborted; ) {
      y++;
      const D = c.headers instanceof Headers ? c.headers : new Headers(c.headers);
      p !== void 0 && D.set("Last-Event-ID", p);
      try {
        const L = {
          redirect: "follow",
          ...c,
          body: c.serializedBody,
          headers: D,
          signal: C
        };
        let B = new Request(d, L);
        e && (B = await e(d, L));
        const M = await (c.fetch ?? globalThis.fetch)(B);
        if (!M.ok) throw new Error(`SSE failed: ${M.status} ${M.statusText}`);
        if (!M.body) throw new Error("No body in SSE response");
        const q = M.body.pipeThrough(new TextDecoderStream()).getReader();
        let x = "";
        const te = () => {
          try {
            q.cancel();
          } catch {
          }
        };
        C.addEventListener("abort", te);
        try {
          for (; ; ) {
            const { done: ae, value: de } = await q.read();
            if (ae) break;
            x += de, x = x.replace(/\r\n?/g, `
`);
            const se = x.split(`

`);
            x = se.pop() ?? "";
            for (const E of se) {
              const I = E.split(`
`), O = [];
              let m;
              for (const v of I)
                if (v.startsWith("data:"))
                  O.push(v.replace(/^data:\s*/, ""));
                else if (v.startsWith("event:"))
                  m = v.replace(/^event:\s*/, "");
                else if (v.startsWith("id:"))
                  p = v.replace(/^id:\s*/, "");
                else if (v.startsWith("retry:")) {
                  const U = Number.parseInt(v.replace(/^retry:\s*/, ""), 10);
                  Number.isNaN(U) || (k = U);
                }
              let w, T = !1;
              if (O.length) {
                const v = O.join(`
`);
                try {
                  w = JSON.parse(v), T = !0;
                } catch {
                  w = v;
                }
              }
              T && (r && await r(w), s && (w = await s(w))), n == null || n({
                data: w,
                event: m,
                id: p,
                retry: k
              }), O.length && (yield w);
            }
          }
        } finally {
          C.removeEventListener("abort", te), q.releaseLock();
        }
        break;
      } catch (L) {
        if (t == null || t(L), l !== void 0 && y >= l)
          break;
        const B = Math.min(k * 2 ** (y - 1), i ?? 3e4);
        await u(B);
      }
    }
  }() };
}
const vy = (e) => {
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
}, Ey = (e) => {
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
}, Sy = (e) => {
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
    const i = (e ? r : r.map((o) => encodeURIComponent(o))).join(Ey(s));
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
  const a = vy(s), l = r.map((i) => s === "label" || s === "simple" ? e ? i : encodeURIComponent(i) : yt({
    allowReserved: e,
    name: n,
    value: i
  })).join(a);
  return s === "label" || s === "matrix" ? a + l : l;
}, yt = ({
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
  valueOnly: a
}) => {
  if (r instanceof Date)
    return a ? r.toISOString() : `${n}=${r.toISOString()}`;
  if (s !== "deepObject" && !t) {
    let o = [];
    Object.entries(r).forEach(([c, p]) => {
      o = [...o, c, e ? p : encodeURIComponent(p)];
    });
    const d = o.join(",");
    switch (s) {
      case "form":
        return `${n}=${d}`;
      case "label":
        return `.${d}`;
      case "matrix":
        return `;${n}=${d}`;
      default:
        return d;
    }
  }
  const l = Sy(s), i = Object.entries(r).map(
    ([o, d]) => yt({
      allowReserved: e,
      name: s === "deepObject" ? `${n}[${o}]` : o,
      value: d
    })
  ).join(l);
  return s === "label" || s === "matrix" ? l + i : i;
}, Ty = /\{[^{}]+\}/g, xy = ({ path: e, url: t }) => {
  let n = t;
  const s = t.match(Ty);
  if (s)
    for (const r of s) {
      let a = !1, l = r.substring(1, r.length - 1), i = "simple";
      l.endsWith("*") && (a = !0, l = l.substring(0, l.length - 1)), l.startsWith(".") ? (l = l.substring(1), i = "label") : l.startsWith(";") && (l = l.substring(1), i = "matrix");
      const o = e[l];
      if (o == null)
        continue;
      if (Array.isArray(o)) {
        n = n.replace(r, Ls({ explode: a, name: l, style: i, value: o }));
        continue;
      }
      if (typeof o == "object") {
        n = n.replace(
          r,
          Bs({
            explode: a,
            name: l,
            style: i,
            value: o,
            valueOnly: !0
          })
        );
        continue;
      }
      if (i === "matrix") {
        n = n.replace(
          r,
          `;${yt({
            name: l,
            value: o
          })}`
        );
        continue;
      }
      const d = encodeURIComponent(
        i === "label" ? `.${o}` : o
      );
      n = n.replace(r, d);
    }
  return n;
}, My = ({
  baseUrl: e,
  path: t,
  query: n,
  querySerializer: s,
  url: r
}) => {
  const a = r.startsWith("/") ? r : `/${r}`;
  let l = (e ?? "") + a;
  t && (l = xy({ path: t, url: l }));
  let i = n ? s(n) : "";
  return i.startsWith("?") && (i = i.substring(1)), i && (l += `?${i}`), l;
};
function An(e) {
  const t = e.body !== void 0;
  if (t && e.bodySerializer)
    return "serializedBody" in e ? e.serializedBody !== void 0 && e.serializedBody !== "" ? e.serializedBody : null : e.body !== "" ? e.body : null;
  if (t)
    return e.body;
}
const Cy = async (e, t) => {
  const n = typeof t == "function" ? await t(e) : t;
  if (n)
    return e.scheme === "bearer" ? `Bearer ${n}` : e.scheme === "basic" ? `Basic ${btoa(n)}` : n;
}, zs = ({
  parameters: e = {},
  ...t
} = {}) => (s) => {
  const r = [];
  if (s && typeof s == "object")
    for (const a in s) {
      const l = s[a];
      if (l == null)
        continue;
      const i = e[a] || t;
      if (Array.isArray(l)) {
        const o = Ls({
          allowReserved: i.allowReserved,
          explode: !0,
          name: a,
          style: "form",
          value: l,
          ...i.array
        });
        o && r.push(o);
      } else if (typeof l == "object") {
        const o = Bs({
          allowReserved: i.allowReserved,
          explode: !0,
          name: a,
          style: "deepObject",
          value: l,
          ...i.object
        });
        o && r.push(o);
      } else {
        const o = yt({
          allowReserved: i.allowReserved,
          name: a,
          value: l
        });
        o && r.push(o);
      }
    }
  return r.join("&");
}, Ay = (e) => {
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
}, Iy = (e, t) => {
  var n, s;
  return t ? !!(e.headers.has(t) || (n = e.query) != null && n[t] || (s = e.headers.get("Cookie")) != null && s.includes(`${t}=`)) : !1;
};
async function Oy(e) {
  for (const t of e.security ?? []) {
    if (Iy(e, t.name))
      continue;
    const n = await Cy(t, e.auth);
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
const In = (e) => My({
  baseUrl: e.baseUrl,
  path: e.path,
  query: e.query,
  querySerializer: typeof e.querySerializer == "function" ? e.querySerializer : zs(e.querySerializer),
  url: e.url
}), On = (e, t) => {
  var s;
  const n = { ...e, ...t };
  return (s = n.baseUrl) != null && s.endsWith("/") && (n.baseUrl = n.baseUrl.substring(0, n.baseUrl.length - 1)), n.headers = Ps(e.headers, t.headers), n;
}, Ny = (e) => {
  const t = [];
  return e.forEach((n, s) => {
    t.push([s, n]);
  }), t;
}, Ps = (...e) => {
  const t = new Headers();
  for (const n of e) {
    if (!n)
      continue;
    const s = n instanceof Headers ? Ny(n) : Object.entries(n);
    for (const [r, a] of s)
      if (a === null)
        t.delete(r);
      else if (Array.isArray(a))
        for (const l of a)
          t.append(r, l);
      else a !== void 0 && t.set(
        r,
        typeof a == "object" ? JSON.stringify(a) : a
      );
  }
  return t;
};
let Tt = class {
  constructor() {
    V(this, "fns", []);
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
const Ry = () => ({
  error: new Tt(),
  request: new Tt(),
  response: new Tt()
}), Ly = zs({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), By = {
  "Content-Type": "application/json"
}, en = (e = {}) => ({
  ...wy,
  headers: By,
  parseAs: "auto",
  querySerializer: Ly,
  ...e
}), Ds = (e = {}) => {
  let t = On(en(), e);
  const n = () => ({ ...t }), s = (c) => (t = On(t, c), n()), r = Ry(), a = async (c) => {
    const p = {
      ...t,
      ...c,
      fetch: c.fetch ?? t.fetch ?? globalThis.fetch,
      headers: Ps(t.headers, c.headers),
      serializedBody: void 0
    };
    p.security && await Oy(p), p.requestValidator && await p.requestValidator(p), p.body !== void 0 && p.bodySerializer && (p.serializedBody = p.bodySerializer(p.body)), (p.body === void 0 || p.serializedBody === "") && p.headers.delete("Content-Type");
    const u = p, _ = In(u);
    return { opts: u, url: _ };
  }, l = async (c) => {
    const p = c.throwOnError ?? t.throwOnError, u = c.responseStyle ?? t.responseStyle;
    let _, f;
    try {
      const { opts: k, url: y } = await a(c), C = {
        redirect: "follow",
        ...k,
        body: An(k)
      };
      _ = new Request(y, C);
      for (const M of r.request.fns)
        M && (_ = await M(_, k));
      const D = k.fetch;
      f = await D(_);
      for (const M of r.response.fns)
        M && (f = await M(f, _, k));
      const L = {
        request: _,
        response: f
      };
      if (f.ok) {
        const M = (k.parseAs === "auto" ? Ay(f.headers.get("Content-Type")) : k.parseAs) ?? "json";
        if (f.status === 204 || f.headers.get("Content-Length") === "0") {
          let x;
          switch (M) {
            case "arrayBuffer":
            case "blob":
            case "text":
              x = await f[M]();
              break;
            case "formData":
              x = new FormData();
              break;
            case "stream":
              x = f.body;
              break;
            case "json":
            default:
              x = {};
              break;
          }
          return k.responseStyle === "data" ? x : {
            data: x,
            ...L
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
            const x = await f.text();
            q = x ? JSON.parse(x) : {};
            break;
          }
          case "stream":
            return k.responseStyle === "data" ? f.body : {
              data: f.body,
              ...L
            };
        }
        return M === "json" && (k.responseValidator && await k.responseValidator(q), k.responseTransformer && (q = await k.responseTransformer(q))), k.responseStyle === "data" ? q : {
          data: q,
          ...L
        };
      }
      const B = await f.text();
      let G;
      try {
        G = JSON.parse(B);
      } catch {
      }
      throw G ?? B;
    } catch (k) {
      let y = k;
      for (const C of r.error.fns)
        C && (y = await C(y, f, _, c));
      if (y = y || {}, p)
        throw y;
      return u === "data" ? void 0 : {
        error: y,
        request: _,
        response: f
      };
    }
  }, i = (c) => (p) => l({ ...p, method: c }), o = (c) => async (p) => {
    const { opts: u, url: _ } = await a(p);
    return $y({
      ...u,
      body: u.body,
      method: c,
      onRequest: async (f, k) => {
        let y = new Request(f, k);
        for (const C of r.request.fns)
          C && (y = await C(y, u));
        return y;
      },
      serializedBody: An(u),
      url: _
    });
  };
  return {
    buildUrl: (c) => In({ ...t, ...c }),
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
      connect: o("CONNECT"),
      delete: o("DELETE"),
      get: o("GET"),
      head: o("HEAD"),
      options: o("OPTIONS"),
      patch: o("PATCH"),
      post: o("POST"),
      put: o("PUT"),
      trace: o("TRACE")
    },
    trace: i("TRACE")
  };
}, xe = Ds(en()), zy = (e) => ((e == null ? void 0 : e.client) ?? xe).get({ url: "/ping", ...e }), Py = (e) => ((e == null ? void 0 : e.client) ?? xe).get({ url: "/models", ...e }), Dy = (e) => ((e == null ? void 0 : e.client) ?? xe).get({ url: "/hooks/", ...e }), Uy = (e) => (e.client ?? xe).post({
  url: "/hooks/",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), qy = (e) => (e.client ?? xe).delete({ url: "/hooks/{id}", ...e }), jy = (e) => ((e == null ? void 0 : e.client) ?? xe).delete({ url: "/config", ...e }), Hy = (e) => ((e == null ? void 0 : e.client) ?? xe).get({ url: "/config", ...e }), Vy = (e) => (e.client ?? xe).post({
  url: "/config",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), Fy = (e) => (e.client ?? xe).post({
  url: "/chat",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), Gy = (e) => (e.client ?? xe).post({
  url: "/agent",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), Wy = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  deleteConfig: jy,
  deleteHooksById: qy,
  getConfig: Hy,
  getHooks: Dy,
  getModels: Py,
  getPing: zy,
  postAgent: Gy,
  postChat: Fy,
  postConfig: Vy,
  postHooks: Uy
}, Symbol.toStringTag, { value: "Module" })), Ky = {
  bodySerializer: (e) => JSON.stringify(e, (t, n) => typeof n == "bigint" ? n.toString() : n)
};
function Zy({
  onRequest: e,
  onSseError: t,
  onSseEvent: n,
  responseTransformer: s,
  responseValidator: r,
  sseDefaultRetryDelay: a,
  sseMaxRetryAttempts: l,
  sseMaxRetryDelay: i,
  sseSleepFn: o,
  url: d,
  ...c
}) {
  let p;
  const u = o ?? ((k) => new Promise((y) => setTimeout(y, k)));
  return { stream: async function* () {
    let k = a ?? 3e3, y = 0;
    const C = c.signal ?? new AbortController().signal;
    for (; !C.aborted; ) {
      y++;
      const D = c.headers instanceof Headers ? c.headers : new Headers(c.headers);
      p !== void 0 && D.set("Last-Event-ID", p);
      try {
        const L = {
          redirect: "follow",
          ...c,
          body: c.serializedBody,
          headers: D,
          signal: C
        };
        let B = new Request(d, L);
        e && (B = await e(d, L));
        const M = await (c.fetch ?? globalThis.fetch)(B);
        if (!M.ok) throw new Error(`SSE failed: ${M.status} ${M.statusText}`);
        if (!M.body) throw new Error("No body in SSE response");
        const q = M.body.pipeThrough(new TextDecoderStream()).getReader();
        let x = "";
        const te = () => {
          try {
            q.cancel();
          } catch {
          }
        };
        C.addEventListener("abort", te);
        try {
          for (; ; ) {
            const { done: ae, value: de } = await q.read();
            if (ae) break;
            x += de, x = x.replace(/\r\n?/g, `
`);
            const se = x.split(`

`);
            x = se.pop() ?? "";
            for (const E of se) {
              const I = E.split(`
`), O = [];
              let m;
              for (const v of I)
                if (v.startsWith("data:"))
                  O.push(v.replace(/^data:\s*/, ""));
                else if (v.startsWith("event:"))
                  m = v.replace(/^event:\s*/, "");
                else if (v.startsWith("id:"))
                  p = v.replace(/^id:\s*/, "");
                else if (v.startsWith("retry:")) {
                  const U = Number.parseInt(v.replace(/^retry:\s*/, ""), 10);
                  Number.isNaN(U) || (k = U);
                }
              let w, T = !1;
              if (O.length) {
                const v = O.join(`
`);
                try {
                  w = JSON.parse(v), T = !0;
                } catch {
                  w = v;
                }
              }
              T && (r && await r(w), s && (w = await s(w))), n == null || n({
                data: w,
                event: m,
                id: p,
                retry: k
              }), O.length && (yield w);
            }
          }
        } finally {
          C.removeEventListener("abort", te), q.releaseLock();
        }
        break;
      } catch (L) {
        if (t == null || t(L), l !== void 0 && y >= l)
          break;
        const B = Math.min(k * 2 ** (y - 1), i ?? 3e4);
        await u(B);
      }
    }
  }() };
}
const Xy = (e) => {
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
}, Yy = (e) => {
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
}, Jy = (e) => {
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
}, Us = ({
  allowReserved: e,
  explode: t,
  name: n,
  style: s,
  value: r
}) => {
  if (!t) {
    const i = (e ? r : r.map((o) => encodeURIComponent(o))).join(Yy(s));
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
  const a = Xy(s), l = r.map((i) => s === "label" || s === "simple" ? e ? i : encodeURIComponent(i) : bt({
    allowReserved: e,
    name: n,
    value: i
  })).join(a);
  return s === "label" || s === "matrix" ? a + l : l;
}, bt = ({
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
}, qs = ({
  allowReserved: e,
  explode: t,
  name: n,
  style: s,
  value: r,
  valueOnly: a
}) => {
  if (r instanceof Date)
    return a ? r.toISOString() : `${n}=${r.toISOString()}`;
  if (s !== "deepObject" && !t) {
    let o = [];
    Object.entries(r).forEach(([c, p]) => {
      o = [...o, c, e ? p : encodeURIComponent(p)];
    });
    const d = o.join(",");
    switch (s) {
      case "form":
        return `${n}=${d}`;
      case "label":
        return `.${d}`;
      case "matrix":
        return `;${n}=${d}`;
      default:
        return d;
    }
  }
  const l = Jy(s), i = Object.entries(r).map(
    ([o, d]) => bt({
      allowReserved: e,
      name: s === "deepObject" ? `${n}[${o}]` : o,
      value: d
    })
  ).join(l);
  return s === "label" || s === "matrix" ? l + i : i;
}, Qy = /\{[^{}]+\}/g, eb = ({ path: e, url: t }) => {
  let n = t;
  const s = t.match(Qy);
  if (s)
    for (const r of s) {
      let a = !1, l = r.substring(1, r.length - 1), i = "simple";
      l.endsWith("*") && (a = !0, l = l.substring(0, l.length - 1)), l.startsWith(".") ? (l = l.substring(1), i = "label") : l.startsWith(";") && (l = l.substring(1), i = "matrix");
      const o = e[l];
      if (o == null)
        continue;
      if (Array.isArray(o)) {
        n = n.replace(r, Us({ explode: a, name: l, style: i, value: o }));
        continue;
      }
      if (typeof o == "object") {
        n = n.replace(
          r,
          qs({
            explode: a,
            name: l,
            style: i,
            value: o,
            valueOnly: !0
          })
        );
        continue;
      }
      if (i === "matrix") {
        n = n.replace(
          r,
          `;${bt({
            name: l,
            value: o
          })}`
        );
        continue;
      }
      const d = encodeURIComponent(
        i === "label" ? `.${o}` : o
      );
      n = n.replace(r, d);
    }
  return n;
}, tb = ({
  baseUrl: e,
  path: t,
  query: n,
  querySerializer: s,
  url: r
}) => {
  const a = r.startsWith("/") ? r : `/${r}`;
  let l = (e ?? "") + a;
  t && (l = eb({ path: t, url: l }));
  let i = n ? s(n) : "";
  return i.startsWith("?") && (i = i.substring(1)), i && (l += `?${i}`), l;
};
function Nn(e) {
  const t = e.body !== void 0;
  if (t && e.bodySerializer)
    return "serializedBody" in e ? e.serializedBody !== void 0 && e.serializedBody !== "" ? e.serializedBody : null : e.body !== "" ? e.body : null;
  if (t)
    return e.body;
}
const nb = async (e, t) => {
  const n = typeof t == "function" ? await t(e) : t;
  if (n)
    return e.scheme === "bearer" ? `Bearer ${n}` : e.scheme === "basic" ? `Basic ${btoa(n)}` : n;
}, js = ({
  parameters: e = {},
  ...t
} = {}) => (s) => {
  const r = [];
  if (s && typeof s == "object")
    for (const a in s) {
      const l = s[a];
      if (l == null)
        continue;
      const i = e[a] || t;
      if (Array.isArray(l)) {
        const o = Us({
          allowReserved: i.allowReserved,
          explode: !0,
          name: a,
          style: "form",
          value: l,
          ...i.array
        });
        o && r.push(o);
      } else if (typeof l == "object") {
        const o = qs({
          allowReserved: i.allowReserved,
          explode: !0,
          name: a,
          style: "deepObject",
          value: l,
          ...i.object
        });
        o && r.push(o);
      } else {
        const o = bt({
          allowReserved: i.allowReserved,
          name: a,
          value: l
        });
        o && r.push(o);
      }
    }
  return r.join("&");
}, sb = (e) => {
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
}, rb = (e, t) => {
  var n, s;
  return t ? !!(e.headers.has(t) || (n = e.query) != null && n[t] || (s = e.headers.get("Cookie")) != null && s.includes(`${t}=`)) : !1;
};
async function ab(e) {
  for (const t of e.security ?? []) {
    if (rb(e, t.name))
      continue;
    const n = await nb(t, e.auth);
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
const Rn = (e) => tb({
  baseUrl: e.baseUrl,
  path: e.path,
  query: e.query,
  querySerializer: typeof e.querySerializer == "function" ? e.querySerializer : js(e.querySerializer),
  url: e.url
}), Ln = (e, t) => {
  var s;
  const n = { ...e, ...t };
  return (s = n.baseUrl) != null && s.endsWith("/") && (n.baseUrl = n.baseUrl.substring(0, n.baseUrl.length - 1)), n.headers = Hs(e.headers, t.headers), n;
}, ob = (e) => {
  const t = [];
  return e.forEach((n, s) => {
    t.push([s, n]);
  }), t;
}, Hs = (...e) => {
  const t = new Headers();
  for (const n of e) {
    if (!n)
      continue;
    const s = n instanceof Headers ? ob(n) : Object.entries(n);
    for (const [r, a] of s)
      if (a === null)
        t.delete(r);
      else if (Array.isArray(a))
        for (const l of a)
          t.append(r, l);
      else a !== void 0 && t.set(
        r,
        typeof a == "object" ? JSON.stringify(a) : a
      );
  }
  return t;
};
class xt {
  constructor() {
    V(this, "fns", []);
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
const ib = () => ({
  error: new xt(),
  request: new xt(),
  response: new xt()
}), lb = js({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), cb = {
  "Content-Type": "application/json"
}, tn = (e = {}) => ({
  ...Ky,
  headers: cb,
  parseAs: "auto",
  querySerializer: lb,
  ...e
}), Vs = (e = {}) => {
  let t = Ln(tn(), e);
  const n = () => ({ ...t }), s = (c) => (t = Ln(t, c), n()), r = ib(), a = async (c) => {
    const p = {
      ...t,
      ...c,
      fetch: c.fetch ?? t.fetch ?? globalThis.fetch,
      headers: Hs(t.headers, c.headers),
      serializedBody: void 0
    };
    p.security && await ab(p), p.requestValidator && await p.requestValidator(p), p.body !== void 0 && p.bodySerializer && (p.serializedBody = p.bodySerializer(p.body)), (p.body === void 0 || p.serializedBody === "") && p.headers.delete("Content-Type");
    const u = p, _ = Rn(u);
    return { opts: u, url: _ };
  }, l = async (c) => {
    const p = c.throwOnError ?? t.throwOnError, u = c.responseStyle ?? t.responseStyle;
    let _, f;
    try {
      const { opts: k, url: y } = await a(c), C = {
        redirect: "follow",
        ...k,
        body: Nn(k)
      };
      _ = new Request(y, C);
      for (const M of r.request.fns)
        M && (_ = await M(_, k));
      const D = k.fetch;
      f = await D(_);
      for (const M of r.response.fns)
        M && (f = await M(f, _, k));
      const L = {
        request: _,
        response: f
      };
      if (f.ok) {
        const M = (k.parseAs === "auto" ? sb(f.headers.get("Content-Type")) : k.parseAs) ?? "json";
        if (f.status === 204 || f.headers.get("Content-Length") === "0") {
          let x;
          switch (M) {
            case "arrayBuffer":
            case "blob":
            case "text":
              x = await f[M]();
              break;
            case "formData":
              x = new FormData();
              break;
            case "stream":
              x = f.body;
              break;
            case "json":
            default:
              x = {};
              break;
          }
          return k.responseStyle === "data" ? x : {
            data: x,
            ...L
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
            const x = await f.text();
            q = x ? JSON.parse(x) : {};
            break;
          }
          case "stream":
            return k.responseStyle === "data" ? f.body : {
              data: f.body,
              ...L
            };
        }
        return M === "json" && (k.responseValidator && await k.responseValidator(q), k.responseTransformer && (q = await k.responseTransformer(q))), k.responseStyle === "data" ? q : {
          data: q,
          ...L
        };
      }
      const B = await f.text();
      let G;
      try {
        G = JSON.parse(B);
      } catch {
      }
      throw G ?? B;
    } catch (k) {
      let y = k;
      for (const C of r.error.fns)
        C && (y = await C(y, f, _, c));
      if (y = y || {}, p)
        throw y;
      return u === "data" ? void 0 : {
        error: y,
        request: _,
        response: f
      };
    }
  }, i = (c) => (p) => l({ ...p, method: c }), o = (c) => async (p) => {
    const { opts: u, url: _ } = await a(p);
    return Zy({
      ...u,
      body: u.body,
      method: c,
      onRequest: async (f, k) => {
        let y = new Request(f, k);
        for (const C of r.request.fns)
          C && (y = await C(y, u));
        return y;
      },
      serializedBody: Nn(u),
      url: _
    });
  };
  return {
    buildUrl: (c) => Rn({ ...t, ...c }),
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
      connect: o("CONNECT"),
      delete: o("DELETE"),
      get: o("GET"),
      head: o("HEAD"),
      options: o("OPTIONS"),
      patch: o("PATCH"),
      post: o("POST"),
      put: o("PUT"),
      trace: o("TRACE")
    },
    trace: i("TRACE")
  };
}, K = Vs(tn()), db = (e) => ((e == null ? void 0 : e.client) ?? K).get({ url: "/catchers", ...e }), ub = (e) => ((e == null ? void 0 : e.client) ?? K).post({
  url: "/catchers",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e == null ? void 0 : e.headers
  }
}), hb = (e) => ((e == null ? void 0 : e.client) ?? K).get({ url: "/catchers/tools", ...e }), pb = (e) => ((e == null ? void 0 : e.client) ?? K).get({ url: "/catchers/read-tools", ...e }), fb = (e) => (e.client ?? K).delete({ url: "/catchers/{id}", ...e }), gb = (e) => (e.client ?? K).get({ url: "/catchers/{id}", ...e }), yb = (e) => (e.client ?? K).put({
  url: "/catchers/{id}",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), bb = (e) => (e.client ?? K).get({ url: "/catchers/{id}/versions", ...e }), mb = (e) => (e.client ?? K).patch({
  url: "/catchers/{id}/enabled",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), kb = (e) => (e.client ?? K).post({ url: "/catchers/{id}/consolidate", ...e }), _b = (e) => (e.client ?? K).post({
  url: "/catchers/{id}/save-as-template",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), wb = (e) => (e.client ?? K).post({ url: "/catchers/{id}/detach-template", ...e }), $b = (e) => ((e == null ? void 0 : e.client) ?? K).get({ url: "/conversations", ...e }), vb = (e) => (e.client ?? K).get({ url: "/conversations/{id}/history", ...e }), Eb = (e) => (e.client ?? K).get({ url: "/conversations/{id}", ...e }), Sb = (e) => (e.client ?? K).get({ url: "/catchers/{catcherId}/facts", ...e }), Tb = (e) => (e.client ?? K).delete({ url: "/facts/{id}", ...e }), xb = (e) => (e.client ?? K).put({
  url: "/facts/{id}",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), Mb = (e) => ((e == null ? void 0 : e.client) ?? K).post({
  url: "/ingest/task-completed",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e == null ? void 0 : e.headers
  }
}), Cb = (e) => ((e == null ? void 0 : e.client) ?? K).post({
  url: "/memory/recall",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e == null ? void 0 : e.headers
  }
}), Ab = (e) => ((e == null ? void 0 : e.client) ?? K).post({
  url: "/memory/save",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e == null ? void 0 : e.headers
  }
}), Ib = (e) => ((e == null ? void 0 : e.client) ?? K).post({
  url: "/memory/read",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e == null ? void 0 : e.headers
  }
}), Ob = (e) => ((e == null ? void 0 : e.client) ?? K).get({ url: "/providers", ...e }), Nb = (e) => (e.client ?? K).get({ url: "/tasks/{id}", ...e }), Rb = (e) => ((e == null ? void 0 : e.client) ?? K).get({ url: "/templates", ...e }), Lb = (e) => (e.client ?? K).delete({ url: "/templates/{id}", ...e }), Bb = (e) => (e.client ?? K).get({ url: "/templates/{id}", ...e }), zb = (e) => (e.client ?? K).get({ url: "/templates/{id}/versions", ...e }), Pb = (e) => (e.client ?? K).post({
  url: "/templates/{id}/versions/{version}/instantiate",
  ...e,
  headers: {
    "Content-Type": "application/json",
    ...e.headers
  }
}), Db = (e) => ((e == null ? void 0 : e.client) ?? K).get({ url: "/traces", ...e }), Ub = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  deleteCatchersById: fb,
  deleteFactsById: Tb,
  deleteTemplatesById: Lb,
  getCatchers: db,
  getCatchersByCatcherIdFacts: Sb,
  getCatchersById: gb,
  getCatchersByIdVersions: bb,
  getCatchersReadTools: pb,
  getCatchersTools: hb,
  getConversations: $b,
  getConversationsById: Eb,
  getConversationsByIdHistory: vb,
  getProviders: Ob,
  getTasksById: Nb,
  getTemplates: Rb,
  getTemplatesById: Bb,
  getTemplatesByIdVersions: zb,
  getTraces: Db,
  patchCatchersByIdEnabled: mb,
  postCatchers: ub,
  postCatchersByIdConsolidate: kb,
  postCatchersByIdDetachTemplate: wb,
  postCatchersByIdSaveAsTemplate: _b,
  postIngestTaskCompleted: Mb,
  postMemoryRead: Ib,
  postMemoryRecall: Cb,
  postMemorySave: Ab,
  postTemplatesByIdVersionsByVersionInstantiate: Pb,
  putCatchersById: yb,
  putFactsById: xb
}, Symbol.toStringTag, { value: "Module" })), Fs = (e) => Jt(_y, Rs(Qt({ baseUrl: e }))), Gs = (e) => Jt(Wy, Ds(en({ baseUrl: e }))), Ws = (e) => Jt(Ub, Vs(tn({ baseUrl: e })));
function Dm(e) {
  return {
    wraith: Fs(e.wraith),
    obsidian: Gs(e.obsidian),
    chronos: Ws(e.chronos)
  };
}
class Um {
  constructor(t) {
    V(this, "wraith");
    this.wraith = Fs(t);
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
class qm {
  constructor(t) {
    V(this, "obsidian");
    this.obsidian = Gs(t);
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
async function qb(e, t) {
  var i;
  const n = new TextEncoder(), s = n.encode(e), r = n.encode(t), a = (i = globalThis.crypto) == null ? void 0 : i.subtle;
  let l;
  if (a) {
    const o = await a.importKey("raw", s, { name: "HMAC", hash: "SHA-256" }, !1, ["sign"]);
    l = new Uint8Array(await a.sign("HMAC", o, r));
  } else
    l = jb(s, r);
  return "sha256=" + [...l].map((o) => o.toString(16).padStart(2, "0")).join("");
}
function jb(e, t) {
  e.length > 64 && (e = Mt(e));
  const n = new Uint8Array(64), s = new Uint8Array(64);
  for (let r = 0; r < 64; r++)
    n[r] = (e[r] ?? 0) ^ 54, s[r] = (e[r] ?? 0) ^ 92;
  return Mt(Bn(s, Mt(Bn(n, t))));
}
function Bn(e, t) {
  const n = new Uint8Array(e.length + t.length);
  return n.set(e), n.set(t, e.length), n;
}
const Hb = new Uint32Array([
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
function Mt(e) {
  const t = e.length, n = new Uint8Array((t + 8 >> 6) + 1 << 6);
  n.set(e), n[t] = 128;
  const s = new DataView(n.buffer);
  s.setUint32(n.length - 8, Math.floor(t / 536870912)), s.setUint32(n.length - 4, t << 3 >>> 0);
  let r = 1779033703, a = 3144134277, l = 1013904242, i = 2773480762, o = 1359893119, d = 2600822924, c = 528734635, p = 1541459225;
  const u = new Uint32Array(64);
  for (let k = 0; k < n.length; k += 64) {
    for (let x = 0; x < 16; x++) u[x] = s.getUint32(k + x * 4);
    for (let x = 16; x < 64; x++) {
      const te = ve(u[x - 15], 7) ^ ve(u[x - 15], 18) ^ u[x - 15] >>> 3, ae = ve(u[x - 2], 17) ^ ve(u[x - 2], 19) ^ u[x - 2] >>> 10;
      u[x] = u[x - 16] + te + u[x - 7] + ae >>> 0;
    }
    let y = r, C = a, D = l, L = i, B = o, G = d, M = c, q = p;
    for (let x = 0; x < 64; x++) {
      const te = ve(B, 6) ^ ve(B, 11) ^ ve(B, 25), ae = B & G ^ ~B & M, de = q + te + ae + Hb[x] + u[x] >>> 0, se = ve(y, 2) ^ ve(y, 13) ^ ve(y, 22), E = y & C ^ y & D ^ C & D, I = se + E >>> 0;
      q = M, M = G, G = B, B = L + de >>> 0, L = D, D = C, C = y, y = de + I >>> 0;
    }
    r = r + y >>> 0, a = a + C >>> 0, l = l + D >>> 0, i = i + L >>> 0, o = o + B >>> 0, d = d + G >>> 0, c = c + M >>> 0, p = p + q >>> 0;
  }
  const _ = new Uint8Array(32), f = new DataView(_.buffer);
  return [r, a, l, i, o, d, c, p].forEach((k, y) => f.setUint32(y * 4, k)), _;
}
const ve = (e, t) => (e >>> t | e << 32 - t) >>> 0;
class jm {
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
  /** Like listProviderModels, but each model carries any stats the provider
   *  exposes (context length, type, quantization, load state). */
  listProviderModelInfo(t) {
    return this.get(`/providers/${t}/model-info`);
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
    return ((s == null ? void 0 : s.data) ?? []).map((r) => typeof r == "string" ? r : r.id).filter((r) => !!r).sort((r, a) => r.localeCompare(a));
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
    const r = JSON.stringify(n), a = await qb(t.secret, r), l = await fetch(`${this.baseUrl}/hooks/${t.id}?mode=${s}`, {
      method: "POST",
      headers: { "Content-Type": "application/json", "X-Hive-Signature": a },
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
      const a = await t.json();
      (r = a.errors) != null && r.length && (n = a.errors.join("; "));
    } catch {
    }
    const s = new Error(n);
    return s.status = t.status, s;
  }
}
class Hm {
  constructor(t = "") {
    // Generated, fully-typed SDK (REST) — used for the newer endpoints. The
    // hand-written fetch/gql helpers below still serve the original methods +
    // GraphQL (which has no generated client).
    V(this, "client");
    this.baseUrl = t, this.client = Ws(t);
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
  /** Queue a full rebuild: drop + recreate the collection at the model's dims and re-embed EVERY fact. Returns immediately (202). */
  async rebuildVectors(t) {
    const n = await fetch(`${this.baseUrl}/catchers/${t}/rebuild-vectors`, { method: "POST" });
    if (!n.ok && n.status !== 202) throw await this.toError(n);
  }
  /** Vector-store health for a bank: fact/vector counts + dims-match. Polled to show rebuild progress. */
  async vectorStatus(t) {
    return this.get(`/catchers/${t}/vector-status`);
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
  /** Ordered user/assistant turns for a session id (the chat history to replay
   *  when re-opening a past conversation). Empty when the session has none. */
  getSessionHistory(t, n = 200) {
    return this.get(
      `/conversations/by-session/${encodeURIComponent(t)}/history?limit=${n}`
    );
  }
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
          items { id catcherId catcherVersion direction accessKey taskId sessionId userId matchedCount topScore durationMs error createdAt recalledContent }
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
      const a = await t.json();
      (r = a.errors) != null && r.length ? n = a.errors.join("; ") : a.error ? n = a.error : a.message && (n = a.message);
    } catch {
    }
    const s = new Error(n);
    return s.status = t.status, s;
  }
  async items(t, n, s) {
    var a;
    return ((a = (await this.gql(t, n))[s]) == null ? void 0 : a.items) ?? [];
  }
  async gql(t, n) {
    var a;
    const s = await fetch(`${this.baseUrl}/graphql`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ query: t, variables: n })
    });
    if (!s.ok) throw new Error(`Chronos GraphQL: HTTP ${s.status} ${s.statusText}`);
    const r = await s.json();
    if ((a = r.errors) != null && a.length) throw new Error(r.errors.map((l) => l.message).join("; "));
    if (r.data === void 0 || r.data === null) throw new Error("Chronos GraphQL: empty response");
    return r.data;
  }
}
function Vb(e, t) {
  let n = e;
  return e.startsWith("/") && (n = window.location.origin + e), n = n.replace(/^http:/, "ws:").replace(/^https:/, "wss:"), t && (n += (n.includes("?") ? "&" : "?") + "token=" + encodeURIComponent(t)), n;
}
class Vm {
  constructor(t) {
    V(this, "socket", null);
    V(this, "opts");
    V(this, "handlers");
    V(this, "reconnectAttempts", 0);
    V(this, "closedByUser", !1);
    V(this, "reconnectTimer", null);
    V(this, "queue", []);
    // sends issued before the socket is open
    V(this, "_state", "closed");
    this.opts = t, this.handlers = t.handlers ?? {};
  }
  get state() {
    return this._state;
  }
  get connected() {
    return this._state === "open";
  }
  open() {
    this.closedByUser = !1, this.connect();
  }
  /** Send one chat turn. Queues if the channel isn't open yet. */
  send(t, n) {
    const s = JSON.stringify({ text: t, sessionId: n == null ? void 0 : n.sessionId, userId: n == null ? void 0 : n.userId });
    this.socket && this._state === "open" ? this.socket.send(s) : this.queue.push(s);
  }
  close() {
    var t;
    this.closedByUser = !0, this.reconnectTimer && (clearTimeout(this.reconnectTimer), this.reconnectTimer = null), (t = this.socket) == null || t.close(), this.socket = null, this.setState("closed");
  }
  // ── internals (hidden socket) ──────────────────────────────────────────────
  setState(t) {
    var n, s;
    this._state !== t && (this._state = t, (s = (n = this.handlers).onState) == null || s.call(n, t));
  }
  connect() {
    var n, s;
    this.setState("connecting");
    let t;
    try {
      t = new WebSocket(Vb(this.opts.url, this.opts.token));
    } catch (r) {
      this.setState("error"), (s = (n = this.handlers).onError) == null || s.call(n, r.message), this.scheduleReconnect();
      return;
    }
    this.socket = t, t.onopen = () => {
      for (this.reconnectAttempts = 0, this.setState("open"); this.queue.length && t.readyState === WebSocket.OPEN; ) t.send(this.queue.shift());
    }, t.onmessage = (r) => {
      var l, i, o, d, c, p, u, _;
      let a;
      try {
        a = JSON.parse(typeof r.data == "string" ? r.data : "");
      } catch {
        return;
      }
      switch (a.type) {
        case "ready":
          (i = (l = this.handlers).onReady) == null || i.call(l, { agentId: a.agentId });
          break;
        case "status":
          (d = (o = this.handlers).onStatus) == null || d.call(o, String(a.state ?? ""));
          break;
        case "reply":
          (p = (c = this.handlers).onReply) == null || p.call(c, { text: a.text ?? null, hadErrors: !!a.hadErrors, error: a.error ?? null, trace: a.trace ?? null });
          break;
        case "error":
          (_ = (u = this.handlers).onError) == null || _.call(u, String(a.message ?? "webchannel error"));
          break;
      }
    }, t.onerror = () => {
      this.setState("error");
    }, t.onclose = () => {
      if (this.socket = null, this.closedByUser) {
        this.setState("closed");
        return;
      }
      this.setState("closed"), this.scheduleReconnect();
    };
  }
  scheduleReconnect() {
    if (this.closedByUser || this.opts.autoReconnect === !1) return;
    const t = Math.min(15e3, 500 * 2 ** this.reconnectAttempts);
    this.reconnectAttempts++, this.reconnectTimer = setTimeout(() => this.connect(), t);
  }
}
const Fm = (e) => e.type === "tool_call", Gm = (e) => e.type === "thought", Wm = (e) => e.type === "finished", Km = (e) => e.type === "failed";
export {
  Pn as Badge,
  Kb as Box,
  Zb as Button,
  _o as Card,
  Xb as Checkbox,
  Hm as ChronosService,
  pm as CodeBlock,
  Om as Collapsible,
  km as ConfirmDialog,
  Em as CopyButton,
  Fe as DEFAULT_THEME,
  tm as Divider,
  _m as Drawer,
  vi as EmptyState,
  um as FilterPills,
  Nm as FlowCanvas,
  Tn as FlowConnector,
  tg as FlowNode,
  vs as Icon,
  em as InfoLabel,
  Tm as JsonViewer,
  Mm as KeyValueList,
  Cm as LoadingState,
  bm as Markdown,
  Gu as Modal,
  $m as ModelSelect,
  vn as NewBadge,
  Rm as NodePalette,
  qm as ObsidianService,
  rm as PageHeader,
  dm as ProgressBar,
  vm as SearchField,
  gm as Segmented,
  Lh as Select,
  hm as SettingRow,
  nm as SidebarNav,
  Jb as Slider,
  jm as SpineService,
  af as Spinner,
  lm as Stat,
  cm as StatStrip,
  Sm as StatusBadge,
  Ao as StatusDot,
  Am as Tabs,
  qp as Tag,
  xm as TagInput,
  ua as TextField,
  fm as TextVarBlock,
  Qb as Textarea,
  om as Tile,
  im as TileGrid,
  wm as ToastHost,
  Yb as Toggle,
  am as ToolRow,
  Im as Tooltip,
  sm as TopBar,
  ym as VarChips,
  Vm as Webchannel,
  Um as WraithService,
  Gg as applyTheme,
  Bm as applyThemeFromImageSrc,
  Ws as chronosClient,
  Lm as clearTheme,
  Dm as createHiveClient,
  Fg as deriveThemeFromImage,
  Cs as exceedsThreshold,
  mm as iconNames,
  Km as isFailed,
  Wm as isFinished,
  Gm as isThought,
  Fm as isToolCall,
  dg as linearize,
  Gs as obsidianClient,
  Vg as rgbToOklch,
  ug as useSortable,
  ph as useToast,
  Fs as wraithClient
};

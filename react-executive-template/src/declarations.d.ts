// Type declaration for the @crestron/ch5-crcomlib CJS bundle.
// The package omits a "main" field so TypeScript cannot locate types
// automatically; this shim re-exports the bundle's own type index.
declare module '@crestron/ch5-crcomlib/build_bundles/cjs/cr-com-lib' {
  type SignalType = 'b' | 'n' | 's';

  interface ICrComLib {
    subscribeState(
      type: 'b',
      join: string,
      callback: (value: boolean) => void,
    ): string;
    subscribeState(
      type: 'n',
      join: string,
      callback: (value: number) => void,
    ): string;
    subscribeState(
      type: 's',
      join: string,
      callback: (value: string) => void,
    ): string;

    unsubscribeState(type: SignalType, join: string, id: string): void;

    publishEvent(type: 'b', join: string, value: boolean): void;
    publishEvent(type: 'n', join: string, value: number): void;
    publishEvent(type: 's', join: string, value: string): void;
  }

  export const CrComLib: ICrComLib;
}

// ── JSX intrinsic element declarations for Crestron CH5 web components ────────

type Ch5ButtonType =
  | 'default' | 'danger' | 'text' | 'warning'
  | 'info' | 'success' | 'primary' | 'secondary';

declare namespace React {
  namespace JSX {
    interface IntrinsicElements {
      /**
       * Crestron CH5 button-list web component.
       *
       * Key join attributes (numeric join numbers supplied as strings):
       *   buttonSendEventOnClick      – base digital join; button N fires join (base + N).
       *   buttonReceiveStateSelected  – base digital join for selected-state feedback.
       *   buttonReceiveStateLabel     – base serial join for per-button label feedback.
       *   buttonReceiveStateEnable    – base digital join for per-button enable feedback.
       */
      'ch5-button-list': React.DetailedHTMLProps<
        React.HTMLAttributes<HTMLElement> & {
          orientation?: 'vertical' | 'horizontal';
          numberOfItems?: number | string;
          buttonType?: Ch5ButtonType;
          centerItems?: boolean | '';
          scrollbar?: boolean | '';
          stretch?: 'both';
          endless?: boolean | '';
          indexId?: string;
          buttonLabelInnerHtml?: string;
          buttonSendEventOnClick?: string;
          buttonReceiveStateSelected?: string;
          buttonReceiveStateLabel?: string;
          buttonReceiveStateEnable?: string;
          buttonReceiveStateShow?: string;
          buttonReceiveStateMode?: string;
          buttonShape?: 'rounded-rectangle' | 'rectangle';
          buttonHAlignLabel?: 'center' | 'left' | 'right';
          loadItems?: 'visible-only' | 'load-new' | 'all';
        },
        HTMLElement
      >;

      /** Label template for all buttons in a ch5-button-list. */
      'ch5-button-list-label': React.DetailedHTMLProps<
        React.HTMLAttributes<HTMLElement>,
        HTMLElement
      >;

      /**
       * Per-button override child of ch5-button-list.
       * Inherits join offsets from the parent list; provides static overrides
       * such as labelInnerHTML.
       */
      'ch5-button-list-individual-button': React.DetailedHTMLProps<
        React.HTMLAttributes<HTMLElement> & {
          labelInnerHTML?: string;
          iconUrl?: string;
          iconClass?: string;
        },
        HTMLElement
      >;
    }
  }
}
